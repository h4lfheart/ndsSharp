using System.Drawing;

namespace ndsSharp.Core.Conversion.Textures.Pixels;

public static class PixelSwizzler
{
    private static readonly Size DefaultTileSize = new(8, 8);

    public static void UnSwizzle<T>(ref T[] input, int width)
    {
        var output = new T[input.Length];
        for (var pixelIndex = 0; pixelIndex < input.Length; pixelIndex++) 
        {
            var tiledIndex = GetTiledIndex(pixelIndex % width, pixelIndex / width, width);
            output[pixelIndex] = tiledIndex < input.Length ? input[tiledIndex] : input[pixelIndex];
        }

        input = output;
    }
    
    private static int GetTiledIndex(int x, int y, int width)
    {
        var tilePixels = DefaultTileSize.Width * DefaultTileSize.Height;
        var horizontalTiles = width / DefaultTileSize.Width;

        var pixelPos = new Point(x % DefaultTileSize.Width, y % DefaultTileSize.Height);
        var tilePos = new Point(x / DefaultTileSize.Width, y / DefaultTileSize.Height);

        var tileRow = tilePos.Y * horizontalTiles * tilePixels;
        var tileColumn = tilePos.X * tilePixels;
        var pixelRow = pixelPos.Y * DefaultTileSize.Width;
        var pixelColumn = pixelPos.X;

        return tileRow + tileColumn + pixelRow + pixelColumn;
    }
}