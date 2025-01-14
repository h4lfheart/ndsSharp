using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ndsSharp.Viewer.Shared.Plugins;

public partial class ViewerPluginFileTypeAssociation : ObservableObject
{
    [ObservableProperty] private Type[] _fileTypes;
    [ObservableProperty] private Type _previewWindowType;

    public ViewerPluginFileTypeAssociation(Type fileType, Type previewWindowType)
    {
        _fileTypes = [fileType];
        _previewWindowType = previewWindowType;
    }
    
    public ViewerPluginFileTypeAssociation(Type[] fileTypes, Type previewWindowType)
    {
        _fileTypes = fileTypes;
        PreviewWindowType = previewWindowType;
    }
}