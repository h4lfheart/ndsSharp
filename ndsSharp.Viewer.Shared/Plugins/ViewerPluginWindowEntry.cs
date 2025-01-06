using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ndsSharp.Core.Providers;
using ndsSharp.Viewer.Shared.Framework;

namespace ndsSharp.Viewer.Shared.Plugins;

public partial class ViewerPluginWindowEntry(string name, string description, Type windowType) : ObservableObject
{
    [ObservableProperty] private string _name = name;
    [ObservableProperty] private string _description = description;
    [ObservableProperty] private Type _windowType = windowType;
}