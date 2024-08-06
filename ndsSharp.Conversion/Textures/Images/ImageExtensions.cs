using ndsSharp.Conversion.Textures.Images.Types;
using ndsSharp.Conversion.Textures.Palettes;
using ndsSharp.Conversion.Textures.Pixels.Colored;
using ndsSharp.Conversion.Textures.Pixels.Indexed;
using ndsSharp.Core.Objects.Exports.Textures;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using IPixel = ndsSharp.Conversion.Textures.Pixels.IPixel;

namespace ndsSharp.Conversion.Textures.Images;

public static class ImageExtensions
{
    public static Image<Rgba32> ToImageSharp(this BaseImage image)
    {
        return image switch
        {
            IndexedPaletteImage indexedPaletteImage => indexedPaletteImage.ToImageSharp(),
            ColoredImage coloredImage => coloredImage.ToImageSharp()
        };
    }
    
    private delegate void PixelRef(ref Rgba32 pixel, int index);
    
    private static void IteratePixels(this Image<Rgba32> image, PixelRef action)
    {
        var pixelIndex = 0;
        image.ProcessPixelRows(accessor =>
        {
            for (var y = 0; y < accessor.Height; y++)
            {
                var pixelRow = accessor.GetRowSpan(y);
                foreach (ref var pixel in pixelRow)
                {
                    action(ref pixel, pixelIndex);
                    pixelIndex++;
                }
            }
        });
    }
    
    private static Image<Rgba32> ToImageSharp(this IndexedPaletteImage image)
    {
        return ToImageSharp(image.Pixels, image.MetaData, image.Palettes);
    }
    
    private static Image<Rgba32> ToImageSharp(this ColoredImage image)
    {
        return ToImageSharp(image.Pixels, image.MetaData);
    }
    
    private static Image<Rgba32> ToImageSharp(IPixel[] pixels, ImageMetaData metaData, List<Palette>? palettes = null)
    {
        palettes ??= [];
        
        var bitmap = new Image<Rgba32>(metaData.Width, metaData.Height);
        bitmap.IteratePixels((ref Rgba32 pixel, int index) =>
        {
            var sourcePixel = pixels[index];

            Rgba32 color;
            switch (sourcePixel)
            {
                case IndexedPixel indexedPixel:
                {
                    var palette = palettes[indexedPixel.PaletteIndex];
                    var pixelColor = palette.Colors[Math.Min(indexedPixel.Index, palette.Colors.Count - 1)]; 
                    
                    color = pixelColor.ToPixel<Rgba32>();
                    if (indexedPixel.Alpha != 255)
                    {
                        color.A = indexedPixel.Alpha;
                    }

                    if (indexedPixel.Index == 0 && metaData.IsFirstColorTransparent)
                    {
                        color.A = 0;
                    }
                    
                    break;
                }
                case ColoredPixel coloredPixel:
                {
                    color = coloredPixel.Color.ToPixel<Rgba32>();
                    break;
                }
                default:
                {
                    color = new Rgba32();
                    break;
                }
            }

            pixel = color;
        });

        return bitmap;
    }
}