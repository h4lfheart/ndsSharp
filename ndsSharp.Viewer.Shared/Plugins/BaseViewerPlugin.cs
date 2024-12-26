using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ndsSharp.Core.Plugins;
using ndsSharp.Viewer.Shared.Framework;
using WindowBase = ndsSharp.Viewer.Shared.Framework.WindowBase;

namespace ndsSharp.Viewer.Shared.Plugins;

public class BaseViewerPlugin<T> : BaseViewerPlugin where T : BasePlugin
{
    public T PluginInterface;
}

public partial class BaseViewerPlugin : ObservableObject
{
    public virtual List<ViewerPluginWindowEntry> PluginWindows { get; } = [];
    public virtual List<ViewerPluginFileTypeAssociation> FileTypeAssociations { get; } = [];
}
