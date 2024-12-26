using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ndsSharp.Viewer.Shared.Framework;

namespace ndsSharp.Viewer.Shared.Plugins;

public partial class ViewerPluginWindowEntry(string name, Type windowType) : ObservableObject
{
    [ObservableProperty] private string _name = name;
    [ObservableProperty] private Type _windowType = windowType;

    [RelayCommand]
    public void OpenWindow()
    {
        if (Activator.CreateInstance(WindowType) is not WindowBase window) return;
        
        window.Show();
        window.BringToTop();
    }
}