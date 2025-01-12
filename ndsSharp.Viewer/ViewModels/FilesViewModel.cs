using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using ndsSharp.Core.Conversion.Models;
using ndsSharp.Core.Conversion.Models.Processing;
using ndsSharp.Core.Conversion.Textures.Images;
using ndsSharp.Core.Objects.Exports.Meshes;
using ndsSharp.Core.Objects.Exports.Textures;
using ndsSharp.Core.Providers;
using ndsSharp.Viewer.Models;
using ndsSharp.Viewer.Models.App;
using ndsSharp.Viewer.Models.Files;
using ndsSharp.Viewer.Services;
using ndsSharp.Viewer.Shared.Extensions;
using ndsSharp.Viewer.Shared.Framework;
using ndsSharp.Viewer.Shared.Plugins;
using ndsSharp.Viewer.Shared.Services;
using ReactiveUI;
using SixLabors.ImageSharp;
using WindowBase = ndsSharp.Viewer.Shared.Framework.WindowBase;

namespace ndsSharp.Viewer.ViewModels;

public partial class FilesViewModel : ViewModelBase
{
    [ObservableProperty] private bool _romLoaded;
    
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
    
    [ObservableProperty] private bool _useRegex;
    
    [ObservableProperty] private List<FlatItem> _selectedFlatViewItems = [];
    [ObservableProperty] private ReadOnlyObservableCollection<FlatItem> _flatViewCollection = new([]);
    private readonly SourceCache<FlatItem, string> _flatViewAssetList = new(item => item.Path);

    [ObservableProperty] private TreeItem? _selectedTreeItem;
    [ObservableProperty] private ObservableCollection<TreeItem> _treeViewCollection = new([]);
    private Dictionary<string, TreeItem> _treeViewBuildHierarchy = [];
    
    
    [RelayCommand]
    public async Task Export()
    {
        var targetItem = SelectedFlatViewItems.FirstOrDefault();
        if (targetItem is null) return;
        
        if (await BrowseFolderDialog() is not { } outPath) return;

        var targetFile = MainWM.Provider.Files[targetItem.Path];
        switch (targetFile.Type)
        {
            case "btx":
            case "nsbtx":
            {
                var btx = MainWM.Provider.LoadObject<BTX>(targetFile);
                foreach (var texture in btx.TextureData.Textures)
                {
                    await texture.ToImage().SaveAsPngAsync(Path.Combine(outPath, $"{texture.Name}.png"));
                }
                
                break;
            }
            case "bmd":
            case "nsbmd":
            {
                var bmd = MainWM.Provider.LoadObject<BMD>(targetFile);
                foreach (var model in bmd.ExtractModels())
                {
                    model.SaveToDirectory(outPath, MeshExportType.OBJ);
                }
                
                break;
            }
            default:
            {
                MainWM.Message("Unsupported Exporter", $"Files with the extension \"{targetFile.Type}\" cannot be exported.");
                
                break;
            }
        }
    }

    [RelayCommand]
    public void Preview()
    {
        var targetItem = SelectedFlatViewItems.FirstOrDefault();
        if (targetItem is null) return;

        // i dont like this but it gets the job done
        
        var foundFileType = false;
        var targetFile = MainWM.Provider.Files[targetItem.Path];
        foreach (var fileTypeAssociation in PluginsVM.PluginFileTypeAssociations)
        {
            if (fileTypeAssociation.Extensions.All(extension => !targetFile.Type.Equals(extension, StringComparison.OrdinalIgnoreCase))) continue;
                    
            if (fileTypeAssociation.PreviewWindowType.BaseType?.GenericTypeArguments.FirstOrDefault() is not { } viewerModelType) continue;
            if (viewerModelType.BaseType?.GenericTypeArguments.FirstOrDefault() is not { } assetType) continue;

            var asset = MainWM.Provider.LoadObject(targetFile, assetType);
            var loadMethod = fileTypeAssociation.PreviewWindowType.GetMethod("Load", 
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            loadMethod?.Invoke(null, [fileTypeAssociation.PreviewWindowType, asset]);
                    
            foundFileType = true;
            break;
        }

        if (!foundFileType)
        {
            MainWM.Message("Unsupported Previewer", $"Files with the extension \"{targetFile.Type}\" cannot be previewed.");
        }
    }

    public void LoadFiles()
    {
        _flatViewAssetList.Clear();
        TreeViewCollection.Clear();
        _treeViewBuildHierarchy.Clear();
        
        foreach (var (_, file) in MainWM.Provider.Files)
        {
            var path = file.Path;
            
            _flatViewAssetList.AddOrUpdate(new FlatItem(path));

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
            .WhenAnyValue(viewModel => viewModel.SearchFilter, viewmodel => viewmodel.UseRegex)
            .Select(CreateAssetFilter);

        //var flatCollection = new ObservableCollection<FlatItem>();
        _flatViewAssetList.Connect()
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

    private Func<FlatItem, bool> CreateAssetFilter((string, bool) items)
    {
        var (filter, useRegex) = items;
        
        if (useRegex)
        {
            return asset => Regex.IsMatch(asset.Path, filter);
        }
        
        return asset => MiscExtensions.Filter(asset.Path, filter);
    }
}