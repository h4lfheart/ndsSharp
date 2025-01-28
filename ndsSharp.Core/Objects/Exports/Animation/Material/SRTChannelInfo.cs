using ndsSharp.Core.Data;

namespace ndsSharp.Core.Objects.Exports.Animation.Material;

public class SRTChannelInfo : BaseDeserializable
{
    public ushort FrameCount;
    public byte Flags;
    public uint Offset;
    
    public override void Deserialize(DataReader reader)
    {
        FrameCount = reader.Read<ushort>();
        reader.Position += 1;
        Flags = reader.Read<byte>();
        Offset = reader.Read<uint>();
    }
}