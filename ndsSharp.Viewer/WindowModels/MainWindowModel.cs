using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using FluentAvalonia.Core;
using ndsSharp.Core.Conversion.Textures.Images;
using ndsSharp.Core.Objects.Exports.Meshes;
using ndsSharp.Core.Objects.Exports.Sounds;
using ndsSharp.Core.Objects.Exports.Textures;
using ndsSharp.Core.Plugins;
using ndsSharp.Core.Providers;
using ndsSharp.Plugins;
using ndsSharp.Viewer.Models;
using ndsSharp.Viewer.Models.Files;
using ndsSharp.Viewer.Services;
using ndsSharp.Viewer.Shared.Extensions;
using ndsSharp.Viewer.Shared.Framework;
using ndsSharp.Viewer.Shared.Plugins;
using ndsSharp.Viewer.Shared.Services;
using ndsSharp.Viewer.Windows;
using ReactiveUI;
using WindowBase = ndsSharp.Viewer.Shared.Framework.WindowBase;

namespace ndsSharp.Viewer.WindowModels;

public partial class MainWindowModel : WindowModelBase
{
    public NdsFileProvider Provider;
    
    // fixes freezes when using ObservableProperty
    private string _searchFilter = string.Empty;

    public string SearchFilter
    {
        get => _searchFilter;
        set
        {
            _searchFilter = value;
            OnPropertyChanged();
        }
    }
    
    [ObservableProperty] private string _titleString = "ndsSharp.Viewer";

    [ObservableProperty] private PropertiesData _properties;
    
    [ObservableProperty] private List<FlatItem> _selectedFlatViewItems = [];
    [ObservableProperty] private ReadOnlyObservableCollection<FlatItem> _flatViewCollection = new([]);

    [ObservableProperty] private TreeItem _selectedTreeItem;
    [ObservableProperty] private ObservableCollection<TreeItem> _treeViewCollection = new([]);

    [ObservableProperty] private ObservableCollection<ViewerPluginWindowEntry> _pluginWindows = [];
    [ObservableProperty] private ObservableCollection<ViewerPluginFileTypeAssociation> _pluginFileTypeAssociations = [];
    
    public readonly SourceCache<FlatItem, string> FlatViewAssetList = new(item => item.Path);
    
    private Dictionary<string, TreeItem> _treeViewBuildHierarchy = [];
    
    [RelayCommand]
    public async Task Open()
    {
        if (await BrowseFileDialog(fileTypes: Globals.RomFileType) is { } romPath)
        {
            LoadRom(romPath);
        }
    }
    
    [RelayCommand]
    public void Exit()
    {
        ApplicationService.Application.Shutdown();
    }

    [RelayCommand]
    public void Export()
    {
        throw new NotImplementedException();
    }

    [RelayCommand]
    public void Preview()
    {
        var targetItem = Enumerable.FirstOrDefault<FlatItem>(SelectedFlatViewItems);
        if (targetItem is null) return;

        var targetFile = Provider.Files[targetItem.Path];
        switch (targetFile.Type)
        {
            case "btx":
            case "nsbtx":
            {
                var btx = Provider.LoadObject<BTX>(targetFile);
                BTXWindow.Create(btx);
                break;
            }
            case "bmd":
            case "nsbmd":
            {
                var bmd = Provider.LoadObject<BMD>(targetFile);
                BMDWindow.Create(bmd);
                break;
            }
            case "strm":
            {
                var strm = Provider.LoadObject<STRM>(targetFile);
                STRMWindow.Create(strm);
                break;
            }
            case "swar":
            {
                var swar = Provider.LoadObject<SWAR>(targetFile);
                SWARWindow.Create(swar);
                break;
            }
            default:
            {
                var foundFileType = false;
                foreach (var fileTypeAssociation in PluginFileTypeAssociations)
                {
                    if (targetFile.Type != fileTypeAssociation.Extension) continue;
                    if (Activator.CreateInstance(fileTypeAssociation.PreviewWindowType) is not WindowBase fileViewer) continue;
                    
                    if (fileViewer.DataContext is not BaseFileViewerModel viewerModel) continue;
                    if (viewerModel.GetType().BaseType?.GenericTypeArguments.FirstOrDefault() is not { } assetType) continue;
                    
                    fileViewer.Show();
                    fileViewer.BringToTop();
                    
                    var asset = Provider.LoadObject(targetFile, assetType);
                    viewerModel.Load(asset);
                    
                    foundFileType = true;
                    break;
                }

                if (!foundFileType)
                {
                    Dialog("Unsupported Previewer", $"Files with the extension \"{targetFile.Type}\" cannot be previewed.");
                }
                
                break;
            }
        }
    }

    [RelayCommand]
    public void OpenPluginWindow(Type windowType)
    {
        var windowObject = Activator.CreateInstance(windowType);
        if (windowObject is not WindowBase window) return;
        if (window.DataContext is not BasePluginWindowModel pluginWindowModel) return;

        pluginWindowModel.Provider = Provider;
        TaskService.Run(pluginWindowModel.Initialize);
        
        window.Show();
        window.BringToTop();
        
    }

    private void LoadRom(string path)
    {
        Provider = new NdsFileProvider(path);
        Provider.UnpackNARCFiles = true;
        Provider.UnpackSDATFiles = true;
        Provider.Initialize();

        LoadPlugins();

        TitleString = $"ndsSharp.Viewer: {Provider.Header.Title}";
        IconSource = new WindowIcon(Provider.Banner.Icon.ToImage().ToWriteableBitmap());
        
        TaskService.Run(LoadFiles);
    }

    private void LoadPlugins()
    {
        Provider.LoadPlugins();
        
        var pluginTypes = Assembly.Load("ndsSharp.Viewer.Plugins").DefinedTypes.Where(type => type.IsAssignableTo(typeof(BaseViewerPlugin)));
        foreach (var pluginType in pluginTypes)
        {
            if (Activator.CreateInstance(pluginType) is not BaseViewerPlugin pluginInstance) continue;
            if (pluginType.BaseType?.GenericTypeArguments.FirstOrDefault() is not { } corePluginType) continue;
            if (Provider.GetPluginInterface(corePluginType) is not { } corePlugin) continue;

            var corePluginField = pluginType.GetField("PluginInterface");
            corePluginField?.SetValue(pluginInstance, corePlugin);
            
            PluginWindows.AddRange(pluginInstance.PluginWindows);
            PluginFileTypeAssociations.AddRange(pluginInstance.FileTypeAssociations);
        }
    }

    private void LoadFiles()
    {
        FlatViewAssetList.Clear();
        TreeViewCollection.Clear();
        _treeViewBuildHierarchy.Clear();
        
        foreach (var (_, file) in Provider.Files)
        {
            var path = file.Path;
            
            FlatViewAssetList.AddOrUpdate(new FlatItem(path));

            var folderNames = path.Split("/", StringSplitOptions.RemoveEmptyEntries);
            var children = _treeViewBuildHierarchy;
            for (var i = 0; i < folderNames.Length; i++)
            {
                var folderName = folderNames[i];
                if (!children.TryGetValue(folderName, out var foundNode))
                {
                    var isFile = i == folderNames.Length - 1;
                    foundNode = new TreeItem(folderName, isFile ? ENodeType.File : ENodeType.Folder,
                        isFile ? path : string.Empty);
                    children.Add(folderName, foundNode);
                }

                children = foundNode.Children;
            }
        }

        void SortChildren(ref Dictionary<string, TreeItem> items)
        {
            items = items
                .OrderBy(item => item.Value.Name, new CustomComparer<string>(ComparisonExtensions.CompareNatural))
                .OrderByDescending(item => item.Value.Type == ENodeType.Folder)
                .ToDictionary();

            foreach (var child in items)
            {
                var tempReference = child.Value.Children;
                SortChildren(ref tempReference);
                child.Value.Children = tempReference;
            }
        }
        
        SortChildren(ref _treeViewBuildHierarchy);
        
        var assetFilter = this
            .WhenAnyValue(viewModel => viewModel.SearchFilter)
            .Select(CreateAssetFilter);

        //var flatCollection = new ObservableCollection<FlatItem>();
        FlatViewAssetList.Connect()
            .ObserveOn(RxApp.TaskpoolScheduler)
            .Filter(assetFilter)
            .Sort(new CustomComparer<FlatItem>((a, b) => ComparisonExtensions.CompareNatural(a.Path, b.Path)))
            .Bind(out var flatCollection)
            .Subscribe();

        FlatViewCollection = flatCollection;
        TreeViewCollection = [.._treeViewBuildHierarchy.Values];

    }
    
    public void FlatViewJumpTo(string directory)
    {
        foreach (var flatItem in FlatViewCollection)
        {
            if (!flatItem.Path.Equals(directory)) continue;

            SelectedFlatViewItems = [flatItem];
            break;
        }
    }
    
    public void TreeViewJumpTo(string directory)
    {
        var i = 0;
        var folders = directory.Split('/');
        var children = _treeViewBuildHierarchy; // start at root
        while (true)
        {
            foreach (var (_, item) in children)
            {
                if (!item.Name.Equals(folders[i], StringComparison.OrdinalIgnoreCase))
                    continue;

                if (item.Type == ENodeType.File)
                {
                    SelectedTreeItem = item;
                    return;
                }

                item.Expanded = true;
                children = item.Children;
                break;
            }

            i++;
            
            if (children.Count == 0) break;
        }
    }

    private Func<FlatItem, bool> CreateAssetFilter(string filter)
    {
        return asset => MiscExtensions.Filter(asset.Path, filter);
    }
    
}