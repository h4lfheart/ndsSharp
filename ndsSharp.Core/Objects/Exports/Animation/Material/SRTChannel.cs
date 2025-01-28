namespace ndsSharp.Core.Objects.Exports.Animation.Material;

public class SRTChannel
{
    public ushort FrameCount;
    public SRTChannelType ChannelType;
    public float[] Samples = [];
}

// TODO rotation and scaling
public enum SRTChannelType
{
    None,
    TranslationU,
    TranslationV
}