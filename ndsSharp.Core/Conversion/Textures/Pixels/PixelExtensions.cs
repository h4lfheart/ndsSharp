using ndsSharp.Core.Conversion.Textures.Images;
using ndsSharp.Core.Conversion.Textures.Images.Types;
using ndsSharp.Core.Data;
using ndsSharp.Core.Objects.Exports.Palettes;
using ndsSharp.Core.Objects.Exports.Textures;

namespace ndsSharp.Core.Conversion.Textures.Pixels;

public static class PixelExtensions
{
    public static IPixel[] ReadPixels<T>(this DataReader reader, int width, int height) where T : PixelDeserializer, new()
    {
        return reader.ReadPixels<T>(width * height);
    }
    
    public static IPixel[] ReadPixels<T>(this DataReader reader, int count) where T : PixelDeserializer, new()
    {
        var deserializer = new T();
        return deserializer.Deserialize(reader, count);
    }
    
    public static IndexedPaletteImage CombineWith(this NCGR ncgr, NCLR nclr, bool isFirstColorTransparent = false)
    {
        var characterData = ncgr.CharacterData;
        
        var meta = new ImageMetaData(
            Width: characterData.Width > 0 ? characterData.Width : 32,
            Height: characterData.Height > 0 ? characterData.Height : characterData.Pixels.Length / 32,
            Format: ncgr.CharacterData.TextureFormat,
            IsFirstColorTransparent: isFirstColorTransparent
        );
        
        return new IndexedPaletteImage(ncgr.File?.Name ?? "Texture", characterData.Pixels, nclr.PaletteData.Palettes, meta);
    }
}