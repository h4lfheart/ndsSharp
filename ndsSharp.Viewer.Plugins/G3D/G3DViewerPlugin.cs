using ndsSharp.Core.Objects.Exports.Meshes;
using ndsSharp.Core.Objects.Exports.Textures;
using ndsSharp.Viewer.Plugins.G3D.FileViewers;
using ndsSharp.Viewer.Shared.Plugins;

namespace ndsSharp.Viewer.Plugins.G3D;

public class G3DViewerPlugin : BaseViewerPlugin
{
    public override List<ViewerPluginFileTypeAssociation> FileTypeAssociations { get; } =
    [
        new([typeof(BMD)], typeof(BMDViewer)),
        new([typeof(BTX)], typeof(BTXViewer))
    ];
}