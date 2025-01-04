using ndsSharp.Viewer.Plugins.Sound.FileViewers;
using ndsSharp.Viewer.Shared.Plugins;

namespace ndsSharp.Viewer.Plugins.Sound;

public class SoundViewerPlugin : BaseViewerPlugin
{
    public override List<ViewerPluginFileTypeAssociation> FileTypeAssociations { get; } =
    [
        new("strm", typeof(STRMViewer)),
        new("swar", typeof(SWARViewer))
    ];
}