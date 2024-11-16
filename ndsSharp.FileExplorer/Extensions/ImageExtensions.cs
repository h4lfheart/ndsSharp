using System;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SkiaSharp;

namespace ndsSharp.FileExplorer.Extensions;

public static class ImageExtensions
{
    public static WriteableBitmap ToWriteableBitmap(this SKBitmap skiaBitmap, bool ignoreAlpha = false)
    {
        using var skiaPixmap = skiaBitmap.PeekPixels();
        using var skiaImage = SKImage.FromPixels(skiaPixmap);
        
        var bitmap = new WriteableBitmap(new PixelSize(skiaBitmap.Width, skiaBitmap.Height), new Vector(96, 96), PixelFormat.Bgra8888, ignoreAlpha ? AlphaFormat.Opaque : AlphaFormat.Unpremul);
        var frameBuffer = bitmap.Lock();

        using (var pixmap = new SKPixmap(new SKImageInfo(skiaBitmap.Width, skiaBitmap.Height, SKColorType.Bgra8888, ignoreAlpha ? SKAlphaType.Opaque : SKAlphaType.Unpremul), frameBuffer.Address, frameBuffer.RowBytes))
        {
            skiaImage.ReadPixels(pixmap, 0, 0);
        }
        
        frameBuffer.Dispose();
        return bitmap;

    }
    
    public static unsafe WriteableBitmap ToWriteableBitmap(this Image<Rgba32> image, bool ignoreAlpha = false)
    {
        var pixelData = new byte[image.Width * image.Height * sizeof(Rgba32)];
        image.CopyPixelDataTo(pixelData);
        
        var bitmap = new WriteableBitmap(new PixelSize(image.Width, image.Height), new Vector(96, 96), PixelFormat.Rgba8888, ignoreAlpha ? AlphaFormat.Opaque : AlphaFormat.Unpremul);

        using var frameBuffer = bitmap.Lock();
        var destPtr = frameBuffer.Address;
        var destStride = frameBuffer.RowBytes;

        fixed (byte* srcPtr = pixelData)
        {
            var dstPtr = (byte*)destPtr;

            // Copy each row of the image data to the WriteableBitmap
            for (var y = 0; y < image.Height; y++)
            {
                Buffer.MemoryCopy(
                    srcPtr + y * image.Width * 4,
                    dstPtr + y * destStride,
                    destStride,
                    image.Width * 4
                );
            }
        }

        return bitmap;

    }
}