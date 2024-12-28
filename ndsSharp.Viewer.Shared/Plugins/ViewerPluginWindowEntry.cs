using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ndsSharp.Core.Providers;
using ndsSharp.Viewer.Shared.Framework;

namespace ndsSharp.Viewer.Shared.Plugins;

public partial class ViewerPluginWindowEntry(string name, Type windowType) : ObservableObject
{
    [ObservableProperty] private string _name = name;
    [ObservableProperty] private Type _windowType = windowType;

}