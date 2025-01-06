using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ndsSharp.Viewer.Shared.Framework;
using ndsSharp.Viewer.Shared.Plugins;
using ndsSharp.Viewer.Shared.Services;

namespace ndsSharp.Viewer.ViewModels;

public partial class PluginsViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<ViewerPluginWindowEntry> _pluginWindows = [];
    [ObservableProperty] private ObservableCollection<ViewerPluginFileTypeAssociation> _pluginFileTypeAssociations = [];
    
    [RelayCommand]
    public void OpenPluginWindow(Type windowType)
    {
        var windowObject = Activator.CreateInstance(windowType);
        if (windowObject is not WindowBase window) return;
        if (window.DataContext is not BasePluginWindowModel pluginWindowModel) return;

        pluginWindowModel.Provider = MainWM.Provider;
        TaskService.Run(pluginWindowModel.Initialize);
        
        window.Show();
        window.BringToTop();
        
    }

}