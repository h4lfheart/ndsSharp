using ndsSharp.Core.Conversion.Textures.Images.Types;
using ndsSharp.Core.Conversion.Textures.Palettes;
using ndsSharp.Core.Conversion.Textures.Pixels.Colored;
using ndsSharp.Core.Conversion.Textures.Pixels.Indexed;
using ndsSharp.Core.Extensions;
using ndsSharp.Core.Objects.Exports.Palettes;
using ndsSharp.Core.Objects.Exports.Textures;
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

    public static Image<Rgba32> ToImage(this NCGR ncgr, NCLR nclr, bool isFirstColorTransparent = false)
    {
        var characterData = ncgr.CharacterData;
        
        var meta = new ImageMetaData(
            Width: characterData.Width > 0 ? characterData.Width : 32,
            Height: characterData.Height > 0 ? characterData.Height : characterData.Pixels.Length / 32,
            Format: ncgr.CharacterData.TextureFormat,
            IsFirstColorTransparent: isFirstColorTransparent
        );
        
        var image = new IndexedPaletteImage(ncgr.File!.Name, characterData.Pixels, nclr.PaletteData.Palettes, meta);
        return image.ToImage();
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
    
    public static Image<Rgba32> ToImage(this AnimatedBannerIcon icon)
    {
        var bitmap = new Image<Rgba32>(icon.Width, icon.Height);
        var rootMetaData = bitmap.Metadata.GetPngMetadata();
        rootMetaData.RepeatCount = 0;
        
        foreach (var key in icon.Keys)
        {
            var targetImage = icon.Images[key.BitmapIndex];
            var image = ToImage(targetImage.Pixels, targetImage.MetaData, [icon.Palettes[key.PaletteIndex]]);
            image.Mutate(ctx =>
            {
                if (key.FlipHorizontal) ctx.Flip(FlipMode.Horizontal);
                if (key.FlipVertical) ctx.Flip(FlipMode.Vertical);
            });
                
            var rootFrame = image.Frames.RootFrame;
            var metaData = rootFrame.Metadata.GetPngMetadata();
            metaData.FrameDelay = new Rational((uint) key.Duration, 1000);

            bitmap.Frames.AddFrame(rootFrame);
        }
        
        bitmap.Frames.RemoveFrame(0);
        return bitmap;
    }
}