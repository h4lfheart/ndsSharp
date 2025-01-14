using ndsSharp.Core.Data;

namespace ndsSharp.Core.Objects.Exports.Palettes.Blocks;

public class PCMP : NdsBlock
{
    public ushort PaletteCount;
    public List<uint> Indices = [];
    
    public override string Magic => "PCMP";

    public override void Deserialize(DataReader reader)
    {
        base.Deserialize(reader);

        PaletteCount = reader.Read<ushort>();
        
        reader.Position += sizeof(ushort);

        var indexTableOffset = reader.Read<uint>();
        reader.Position = (int) indexTableOffset;
        
        for (var i = 0; i < PaletteCount; i++)
        {
            Indices.Add(reader.Read<ushort>());
        }
    }
}