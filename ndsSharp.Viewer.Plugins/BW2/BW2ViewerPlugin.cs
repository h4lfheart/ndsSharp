using ndsSharp.Plugins.BW2;
using ndsSharp.Viewer.Plugins.BW2.Windows;
using ndsSharp.Viewer.Shared.Plugins;
using BW2TextViewer = ndsSharp.Viewer.Plugins.BW2.FileViewers.BW2TextViewer;

namespace ndsSharp.Viewer.Plugins.BW2;

public class BW2ViewerPlugin : BaseViewerPlugin<BW2Plugin>
{
    public override List<ViewerPluginWindowEntry> PluginWindows { get; } = 
    [
        new("Matrix Viewer", typeof(BW2MatrixWindow))
    ];

    public override List<ViewerPluginFileTypeAssociation> FileTypeAssociations { get; } =
    [
        new("text", typeof(BW2TextViewer))
    ];
}