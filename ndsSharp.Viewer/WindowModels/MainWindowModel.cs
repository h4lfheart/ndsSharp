using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using FluentAvalonia.Core;
using FluentAvalonia.UI.Controls;
using ndsSharp.Core.Conversion.Models;
using ndsSharp.Core.Conversion.Models.Processing;
using ndsSharp.Core.Conversion.Textures.Images;
using ndsSharp.Core.Objects.Exports.Meshes;
using ndsSharp.Core.Objects.Exports.Sounds;
using ndsSharp.Core.Objects.Exports.Textures;
using ndsSharp.Core.Plugins;
using ndsSharp.Core.Providers;
using ndsSharp.Plugins;
using ndsSharp.Viewer.Models;
using ndsSharp.Viewer.Models.App;
using ndsSharp.Viewer.Models.Files;
using ndsSharp.Viewer.Services;
using ndsSharp.Viewer.Shared.Extensions;
using ndsSharp.Viewer.Shared.Framework;
using ndsSharp.Viewer.Shared.Plugins;
using ndsSharp.Viewer.Shared.Services;
using ndsSharp.Viewer.Windows;
using ReactiveUI;
using SixLabors.ImageSharp;
using ImageExtensions = ndsSharp.Viewer.Shared.Extensions.ImageExtensions;
using Path = System.IO.Path;
using WindowBase = ndsSharp.Viewer.Shared.Framework.WindowBase;

namespace ndsSharp.Viewer.WindowModels;

public partial class MainWindowModel : WindowModelBase
{
    [ObservableProperty] private NdsFileProvider _provider;
    
    [ObservableProperty] private Frame _contentFrame;
    [ObservableProperty] private NavigationView _navigationView;

    [ObservableProperty] private Bitmap _iconBitmap = ImageExtensions.AvaresBitmap("avares://ndsSharp.Viewer/Assets/icon.png");
        
    [ObservableProperty] private ObservableCollection<InfoBarData> _infoBars = [];
    
    public void LoadFiles(string romPath)
    {
        Provider = new NdsFileProvider(romPath)
        {
            UnpackNARCFiles = true,
            UnpackSDATFiles = true
        };
        
        Provider.Initialize();
        LoadPlugins();
        TaskService.Run(FilesVM.LoadFiles);
        
        TitleString = $"ndsSharp.Viewer: {Provider.Header.Title}";

        IconBitmap = Provider.Banner.Icon.ToImage().ToWriteableBitmap();
        IconSource = new WindowIcon(IconBitmap);

        FilesVM.RomLoaded = true;
    }

    private void LoadPlugins()
    {
        Provider.LoadPlugins();
        
        PluginsVM.PluginWindows.Clear();
        
        var pluginTypes = Assembly.Load("ndsSharp.Viewer.Plugins").DefinedTypes.Where(type => type.IsAssignableTo(typeof(BaseViewerPlugin)));
        foreach (var pluginType in pluginTypes)
        {
            if (Activator.CreateInstance(pluginType) is not BaseViewerPlugin pluginInstance) continue;
            if (pluginType.BaseType?.GenericTypeArguments.FirstOrDefault() is { } corePluginType)
            {
                if (Provider.GetPluginInterface(corePluginType) is not { } corePlugin) continue;

                var corePluginField = pluginType.GetField("PluginInterface");
                corePluginField?.SetValue(pluginInstance, corePlugin);
            }
            
            PluginsVM.PluginWindows.AddRange(pluginInstance.PluginWindows);
            PluginsVM.PluginFileTypeAssociations.AddRange(pluginInstance.FileTypeAssociations);
        }
    }
    
    public void Navigate<T>()
    {
        Navigate(typeof(T));
    }
    
    public void Navigate(Type type)
    {
        ContentFrame.Navigate(type, null, null);

        var buttonName = type.Name.Replace("View", string.Empty);
        NavigationView.SelectedItem = NavigationView.MenuItems
            .Concat(NavigationView.FooterMenuItems)
            .OfType<NavigationViewItem>()
            .FirstOrDefault(item => (string) item.Tag! == buttonName);
    }
    
    public void Message(string title, string message, InfoBarSeverity severity = InfoBarSeverity.Informational, bool autoClose = true, string id = "", float closeTime = 3f, bool useButton = false, string buttonTitle = "", Action? buttonCommand = null)
    {
        Message(new InfoBarData(title, message, severity, autoClose, id, closeTime, useButton, buttonTitle, buttonCommand));
    }

    public void Message(InfoBarData data)
    {
        if (!string.IsNullOrEmpty(data.Id))
            InfoBars.RemoveAll(bar => bar.Id.Equals(data.Id));
        
        InfoBars.Add(data);
        if (!data.AutoClose) return;
        
        TaskService.Run(async () =>
        {
            await Task.Delay((int) (data.CloseTime * 1000));
            InfoBars.Remove(data);
        });
    }
    
    public void UpdateMessage(string id, string message)
    {
        var foundInfoBar = InfoBars.FirstOrDefault(infoBar => infoBar.Id == id);
        if (foundInfoBar is null) return;
        
        foundInfoBar.Message = message;
    }
    
    public void CloseMessage(string id)
    {
        InfoBars.RemoveAll(info => info.Id == id);
    }
    
}