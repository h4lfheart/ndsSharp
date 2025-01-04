using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ndsSharp.Viewer.Shared.Plugins;

public partial class ViewerPluginFileTypeAssociation : ObservableObject
{
    [ObservableProperty] private string[] _extensions;
    [ObservableProperty] private Type _previewWindowType;

    public ViewerPluginFileTypeAssociation(string extension, Type previewWindowType)
    {
        Extensions = [extension];
        PreviewWindowType = previewWindowType;
    }
    
    public ViewerPluginFileTypeAssociation(string[] extensions, Type previewWindowType)
    {
        Extensions = extensions;
        PreviewWindowType = previewWindowType;
    }
}