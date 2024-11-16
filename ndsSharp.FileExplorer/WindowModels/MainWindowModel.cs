using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using ndsSharp.Core.Conversion.Textures.Images;
using ndsSharp.Core.Objects.Exports.Meshes;
using ndsSharp.Core.Objects.Exports.Textures;
using ndsSharp.Core.Providers;
using ndsSharp.FileExplorer.Extensions;
using ndsSharp.FileExplorer.Framework;
using ndsSharp.FileExplorer.Models;
using ndsSharp.FileExplorer.Models.Files;
using ndsSharp.FileExplorer.Rendering.Rendering;
using ndsSharp.FileExplorer.Services;
using ndsSharp.FileExplorer.Windows;
using OpenTK.Mathematics;
using ReactiveUI;
using Serilog;
using SixLabors.ImageSharp;

namespace ndsSharp.FileExplorer.WindowModels;

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
    
    [ObservableProperty] private string _titleString = "ndsViewer";

    [ObservableProperty] private PropertiesData _properties;
    
    [ObservableProperty] private List<FlatItem> _selectedFlatViewItems = [];
    [ObservableProperty] private ReadOnlyObservableCollection<FlatItem> _flatViewCollection = new([]);

    [ObservableProperty] private TreeItem _selectedTreeItem;
    [ObservableProperty] private ObservableCollection<TreeItem> _treeViewCollection = new([]);
    
    public readonly SourceCache<FlatItem, string> FlatViewAssetList = new(item => item.Path);
    
    private Dictionary<string, TreeItem> _treeViewBuildHierarchy = [];

    public MainWindowModel()
    {
#if DEBUG
        LoadRom("C:/b2.nds");
#endif
    }
    
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
        var targetItem = SelectedFlatViewItems.FirstOrDefault();
        if (targetItem is null) return;

        var targetFile = Provider.Files[targetItem.Path];
        switch (targetFile.Type)
        {
            case "btx0":
            {
                var btx = Provider.LoadObject<BTX0>(targetFile);
                BTXWindow.Create(btx);
                break;
            }
            case "bmd0":
            {
                var bmd = Provider.LoadObject<BMD0>(targetFile);
                BMDWindow.Create(bmd);
                break;
            }
        }
    }

    private void LoadRom(string path)
    {
        Provider = new NdsFileProvider(path);
        Provider.UnpackNARCFiles = true;
        Provider.Initialize();

        TitleString = $"ndsViewer: {Provider.Header.Title}";
        IconSource = new WindowIcon(Provider.Banner.Icon.ToImage().ToWriteableBitmap());
        
        TaskService.Run(LoadFiles);
    }

    private void LoadFiles()
    {
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