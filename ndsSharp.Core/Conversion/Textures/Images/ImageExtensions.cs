using ndsSharp.Core.Conversion.Textures.Images.Types;
using ndsSharp.Core.Conversion.Textures.Palettes;
using ndsSharp.Core.Conversion.Textures.Pixels.Colored;
using ndsSharp.Core.Conversion.Textures.Pixels.Indexed;
using ndsSharp.Core.Objects.Rom;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using IPixel = ndsSharp.Core.Conversion.Textures.Pixels.IPixel;

namespace ndsSharp.Core.Conversion.Textures.Images;

public static class ImageExtensions
{
    public static Image<Rgba32> ToImage(this BaseImage image)
    {
        return image switch
        {
            IndexedPaletteImage indexedPaletteImage => indexedPaletteImage.ToImage(),
            ColoredImage coloredImage => coloredImage.ToImage()
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
    
    private static Image<Rgba32> ToImage(this IndexedPaletteImage image)
    {
        return ToImage(image.Pixels, image.MetaData, image.Palettes);
    }
    
    private static Image<Rgba32> ToImage(this ColoredImage image)
    {
        return ToImage(image.Pixels, image.MetaData);
    }
    
    private static Image<Rgba32> ToImage(IPixel[] pixels, ImageMetaData metaData, List<Palette>? palettes = null)
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
                    var pixelColor = palette.Colors[System.Math.Min(indexedPixel.Index, palette.Colors.Count - 1)]; 
                    
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