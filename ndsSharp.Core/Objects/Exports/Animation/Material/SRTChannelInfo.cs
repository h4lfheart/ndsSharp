using ndsSharp.Core.Data;

namespace ndsSharp.Core.Objects.Exports.Animation.Material;

public class SRTChannelInfo : BaseDeserializable
{
    public ushort FrameCount;
    public ushort Flags;
    public uint OffsetOrConst;
    
    public override void Deserialize(DataReader reader)
    {
        FrameCount = reader.Read<ushort>();
        Flags = reader.Read<ushort>();
        OffsetOrConst = reader.Read<uint>();
    }
}