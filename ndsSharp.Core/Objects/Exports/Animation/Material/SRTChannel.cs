namespace ndsSharp.Core.Objects.Exports.Animation.Material;

public class SRTChannel
{
    public ushort FrameCount;
    public float[] Samples = [];
}

public enum SRTChannelType
{
    ScaleU = 0,
    ScaleV = 1,
    Rotation = 2,
    TranslationU = 3,
    TranslationV = 4
}