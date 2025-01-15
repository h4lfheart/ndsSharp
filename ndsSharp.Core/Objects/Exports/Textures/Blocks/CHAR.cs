using System.Diagnostics;
using ndsSharp.Core.Conversion.Textures.Images;
using ndsSharp.Core.Conversion.Textures.Images.Types;
using ndsSharp.Core.Conversion.Textures.Pixels;
using ndsSharp.Core.Conversion.Textures.Pixels.Colored.Types;
using ndsSharp.Core.Conversion.Textures.Pixels.Indexed.Types;
using ndsSharp.Core.Data;
using ndsSharp.Core.Extensions;

namespace ndsSharp.Core.Objects.Exports.Textures.Blocks;

public class CHAR : NdsBlock
{
    public short Width;
    public short Height;
    public TextureFormat TextureFormat;
    public CharacterFormat CharacterFormat;
    public bool IsSwizzled;

    public IPixel[] Pixels = [];
    
    public override string Magic => "CHAR";

    public override void Deserialize(DataReader reader)
    {
        base.Deserialize(reader);

        Width = reader.Read<short>();
        Height = reader.Read<short>();
        TextureFormat = reader.ReadEnum<TextureFormat, uint>();

        CharacterFormat = reader.Read<CharacterFormat>();

        reader.Position += sizeof(ushort);

        var flags = reader.Read<uint>();
        IsSwizzled = (flags & 1) == 1;
        
        var dataLength = reader.Read<uint>();
        
        var dataOffset = reader.Read<uint>();
        reader.Position = (int) dataOffset;

        var pixelCount = (int) (dataLength * 8 / TextureFormat.BitsPerPixel());
        Pixels = TextureFormat switch
        {
            TextureFormat.Color4 => reader.ReadPixels<Indexed2>(pixelCount),
            TextureFormat.Color16 => reader.ReadPixels<Indexed4>(pixelCount),
            TextureFormat.Color256 => reader.ReadPixels<Indexed8>(pixelCount),
            TextureFormat.A3I5 => reader.ReadPixels<A3I5>(pixelCount),
            TextureFormat.A5I3 => reader.ReadPixels<A5I3>(pixelCount),
            TextureFormat.A1BGR5 => reader.ReadPixels<A1BGR555>(pixelCount)
        };

        if (IsSwizzled)
        {
            if (Width == -1) throw new NotSupportedException("Cannot un-swizzle texture with a width of -1");
            
            PixelSwizzler.UnSwizzle(ref Pixels, Width * 8);
        }
    }
}

public enum CharacterFormat : ushort
{
    Character2D = 0,
    Character1D_32K = 16,
    Character1D_64K = 17,
    Character1D_128K = 18,
    Character1D_256K = 19
}