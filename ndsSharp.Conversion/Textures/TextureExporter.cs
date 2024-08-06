using ndsSharp.Conversion.Textures.Colors;
using ndsSharp.Conversion.Textures.Colors.Types;
using ndsSharp.Conversion.Textures.Images;
using ndsSharp.Conversion.Textures.Images.Types;
using ndsSharp.Conversion.Textures.Palettes;
using ndsSharp.Conversion.Textures.Pixels;
using ndsSharp.Conversion.Textures.Pixels.Colored;
using ndsSharp.Conversion.Textures.Pixels.Colored.Types;
using ndsSharp.Conversion.Textures.Pixels.Indexed;
using ndsSharp.Conversion.Textures.Pixels.Indexed.Types;
using ndsSharp.Core.Extensions;
using ndsSharp.Core.Objects.Exports.Textures;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using IPixel = ndsSharp.Conversion.Textures.Pixels.IPixel;

namespace ndsSharp.Conversion.Textures;

public class TextureExporter
{
    private List<BaseImage> _images = [];
    
    public TextureExporter(TEX0 textureData)
    {
        for (var textureIndex = 0; textureIndex < textureData.TextureInfos.Count; textureIndex++)
        {
            var (textureName, texturePointer) = textureData.TexturePointers.ElementAt(textureIndex);
            var textureInfo = textureData.TextureInfos[textureName];
            
            var (paletteName, palettePointer) = textureData.PalettePointers.FirstOrDefault(pair => pair.Key.Equals(textureName + "_pl"), textureData.PalettePointers.ElementAt(Math.Min(textureIndex, textureData.PalettePointers.Count - 1)));
            var paletteReader = palettePointer.Load();
            var palette = new Palette(paletteName, paletteReader.ReadColors<BGR555>());
            
            var pixelReader = texturePointer.Load();
            var pixels = textureInfo.Format switch
            {
                TextureFormat.Color4 => pixelReader.ReadPixels<Indexed2>(textureInfo.Width, textureInfo.Height),
                TextureFormat.Color16 => pixelReader.ReadPixels<Indexed4>(textureInfo.Width, textureInfo.Height),
                TextureFormat.Color256 => pixelReader.ReadPixels<Indexed8>(textureInfo.Width, textureInfo.Height),
                TextureFormat.A3I5 => pixelReader.ReadPixels<A3I5>(textureInfo.Width, textureInfo.Height),
                TextureFormat.A5I3 => pixelReader.ReadPixels<A5I3>(textureInfo.Width, textureInfo.Height),
                TextureFormat.A1BGR5 => pixelReader.ReadPixels<A1BGR555>(textureInfo.Width, textureInfo.Height)
            };
            
            var meta = new ImageMetaData(textureInfo.Width, textureInfo.Height, textureInfo.Format, 
                textureInfo.RepeatU, textureInfo.RepeatV, textureInfo.FlipU, textureInfo.FlipV, 
                textureInfo.FirstColorIsTransparent);

            if (textureInfo.Format.IsIndexed())
            {
                _images.Add(new IndexedPaletteImage(textureName, pixels, [palette], meta));
            }
            else
            {
                _images.Add(new ColoredImage(textureName, pixels, meta));
            }
        }
        
    }

    public void Export(string path)
    {
        foreach (var image in _images)
        {
            var outPath = Path.Combine(path, image.Name + ".png");
            image.ToImageSharp().SaveAsPng(outPath);
        }
    }
    
}