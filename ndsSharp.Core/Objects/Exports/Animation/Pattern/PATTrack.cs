using ndsSharp.Core.Data;

namespace ndsSharp.Core.Objects.Exports.Animation.Pattern;

public class PATTrack
{
    public string Name;
    public uint KeyframeCount;
    public PATKeyframe[] Keyframes = [];
}