using ndsSharp.Conversion.Textures.Palettes;
using ndsSharp.Conversion.Textures.Pixels;

namespace ndsSharp.Conversion.Textures.Images.Types;

public class IndexedPaletteImage : BaseImage
{
    public bool IsFirstColorTransparent;
    public List<Palette> Palettes;
    
    public IndexedPaletteImage(string name, IPixel[] pixels, List<Palette> palettes, ImageMetaData metaData, bool isFirstColorTransparent = false) : base(name, pixels, metaData)
    {
        Palettes = palettes;
        IsFirstColorTransparent = isFirstColorTransparent;
    }
    
    public IndexedPaletteImage(string name, IPixel[] pixels, ImageMetaData metaData) : base(name, pixels, metaData)
    {
        Palettes = new List<Palette>();
        IsFirstColorTransparent = false;
    }
    
    public IndexedPaletteImage(IndexedImage image, Palette palette, bool isFirstColorTransparent = true) : base(image.Name, image.Pixels, image.MetaData)
    {
        Palettes = [palette];
        IsFirstColorTransparent = isFirstColorTransparent;
    }
}