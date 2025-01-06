using ndsSharp.Viewer.Plugins.G3D.FileViewers;
using ndsSharp.Viewer.Shared.Plugins;

namespace ndsSharp.Viewer.Plugins.G3D;

public class G3DViewerPlugin : BaseViewerPlugin
{
    public override List<ViewerPluginFileTypeAssociation> FileTypeAssociations { get; } =
    [
        new(["bmd", "nsbmd"], typeof(BMDViewer)),
        new(["btx", "nsbtx"], typeof(BTXViewer))
    ];
}