using ndsSharp.Core.Data;

namespace ndsSharp.Core.Objects.Exports.Animation.Pattern;

public class PATTrackInfo : BaseDeserializable
{
    public uint KeyframeCount;
    public ushort Flags;
    public ushort Offset;
    
    public override void Deserialize(DataReader reader)
    {
        KeyframeCount = reader.Read<uint>();
        Flags = reader.Read<ushort>();
        Offset = reader.Read<ushort>();
    }
}