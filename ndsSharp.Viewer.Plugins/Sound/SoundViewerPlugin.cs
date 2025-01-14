using ndsSharp.Core.Objects.Exports.Sounds;
using ndsSharp.Viewer.Plugins.Sound.FileViewers;
using ndsSharp.Viewer.Shared.Plugins;

namespace ndsSharp.Viewer.Plugins.Sound;

public class SoundViewerPlugin : BaseViewerPlugin
{
    public override List<ViewerPluginFileTypeAssociation> FileTypeAssociations { get; } =
    [
        new(typeof(STRM), typeof(STRMViewer)),
        new(typeof(SWAR), typeof(SWARViewer))
    ];
}