using ndsSharp.Core.Conversion.Textures.Palettes;
using ndsSharp.Core.Conversion.Textures.Pixels;

namespace ndsSharp.Core.Conversion.Textures.Images.Types;

public class IndexedPaletteImage : BaseImage
{
    public List<Palette> Palettes;
    
    public IndexedPaletteImage(string name, IPixel[] pixels, List<Palette> palettes, ImageMetaData metaData) : base(name, pixels, metaData)
    {
        Palettes = palettes;
    }
    
    public IndexedPaletteImage(string name, IPixel[] pixels, ImageMetaData metaData) : base(name, pixels, metaData)
    {
        Palettes = [];
    }
    
    public IndexedPaletteImage(IndexedImage image, Palette palette) : base(image.Name, image.Pixels, image.MetaData)
    {
        Palettes = [palette];
    }
}