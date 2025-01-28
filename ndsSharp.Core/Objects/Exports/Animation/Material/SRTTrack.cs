namespace ndsSharp.Core.Objects.Exports.Animation.Material;

public class SRTTrack
{
    public string Name;
    public Dictionary<SRTChannelType, SRTChannel> Channels = new();
}