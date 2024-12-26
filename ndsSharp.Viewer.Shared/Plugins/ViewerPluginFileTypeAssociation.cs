using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ndsSharp.Viewer.Shared.Plugins;

public partial class ViewerPluginFileTypeAssociation(string extension, Type previewWindowType) : ObservableObject
{
    [ObservableProperty] private string _extension = extension;
    [ObservableProperty] private Type _previewWindowType = previewWindowType;
}