using ndsSharp.Core.Data;
using ndsSharp.Core.Extensions;

namespace ndsSharp.Core.Objects.Exports.Screen.Blocks;

public class SCRN : NdsBlock
{
    public ushort Width;
    public ushort Height;

    public SCRNTile[] Tiles = [];
    
    public override string Magic => "SCRN";

    public override void Deserialize(DataReader reader)
    {
        base.Deserialize(reader);

        Width = reader.Read<ushort>();
        Height = reader.Read<ushort>();
        reader.Position += sizeof(ushort) * 2;
        reader.Position += sizeof(uint);
        Tiles = reader.ReadArray((Width / 8) * (Height / 8), () => reader.ReadObject<SCRNTile>());
    }
}

public class SCRNTile : BaseDeserializable
{
    public ushort TileIndex;
    public bool FlipU;
    public bool FlipV;
    public byte PaletteIndex;
    
    public override void Deserialize(DataReader reader)
    {
        var flags = reader.Read<ushort>();

        TileIndex = flags.Bits(0, 10);
        FlipU = flags.Bit(10) == 1;
        FlipV = flags.Bit(11) == 1;
        PaletteIndex = (byte) flags.Bits(12, 15);
    }
}