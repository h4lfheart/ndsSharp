using ndsSharp.Core.Data;

namespace ndsSharp.Core.Objects.Exports.Animation.Pattern;

public class PATKeyframe : BaseDeserializable
{
    public ushort Frame;
    public byte TextureIndex;
    public byte PaletteIndex;
    
    public override void Deserialize(DataReader reader)
    {
        Frame = reader.Read<ushort>();
        TextureIndex = reader.Read<byte>();
        PaletteIndex = reader.Read<byte>();
    }
}