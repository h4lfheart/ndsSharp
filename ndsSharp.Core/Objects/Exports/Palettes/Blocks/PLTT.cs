using ndsSharp.Core.Conversion.Textures.Colors;
using ndsSharp.Core.Conversion.Textures.Colors.Types;
using ndsSharp.Core.Conversion.Textures.Palettes;
using ndsSharp.Core.Data;
using ndsSharp.Core.Objects.Exports.Textures.Blocks;

namespace ndsSharp.Core.Objects.Exports.Palettes.Blocks;

public class PLTT : NdsBlock
{
    public TextureFormat Format;
    public List<Palette> Palettes = [];
    
    public override string Magic => "PLTT";

    public override void Deserialize(DataReader reader)
    {
        base.Deserialize(reader);

        Format = reader.ReadEnum<TextureFormat, ushort>();

        reader.Position += 6;

        var dataLength = reader.Read<uint>();
        
        var dataOffset = reader.Read<uint>();
        reader.Position = (int) dataOffset;

        var colorCount = Format switch
        {
            TextureFormat.Color16 => 16,
            TextureFormat.Color256 => 256,
            _ => throw new NotSupportedException($"Invalid palette color type of {Format}.")
        };
        
        var paletteCount = dataLength / (colorCount * 2);
        for (var paletteIndex = 0; paletteIndex < paletteCount; paletteIndex++)
        {
            var colors = reader.ReadColors<BGR555>(colorCount);
            Palettes.Add(new Palette($"Palette_{paletteIndex}", colors));
        }
    }
}