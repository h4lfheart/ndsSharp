using System.Diagnostics;
using ndsSharp.Core.Conversion.Textures.Images;
using ndsSharp.Core.Conversion.Textures.Images.Types;
using ndsSharp.Core.Conversion.Textures.Pixels;
using ndsSharp.Core.Conversion.Textures.Pixels.Indexed;
using ndsSharp.Core.Objects.Exports.Cells;
using ndsSharp.Core.Objects.Exports.Palettes;
using ndsSharp.Core.Objects.Exports.Screen;
using ndsSharp.Core.Objects.Exports.Textures;

namespace ndsSharp.Core.Conversion.Textures.Screen;

public static class ScreenExtensions
{
    public static IndexedPaletteImage ExtractScreenImage(this NSCR screen, NCGR graphic, NCLR palette, int? paletteOverride = null, bool firstColorIsTransparent = true)
    {
        var xTiles = graphic.CharacterData.Width / 8;
        var yTiles = graphic.CharacterData.Height / 8;
        
        var graphicTiles = new List<IPixel[]>();
        for (var yTile = 0; yTile < yTiles; yTile++)
        {
            for (var xTile = 0; xTile < xTiles; xTile++)
            {
                var tilePixels = new IPixel[64];
                for (var y = 0; y < 8; y++)
                {
                    for (var x = 0; x < 8; x++)
                    {
                        tilePixels[y * 8 + x] = graphic.CharacterData.Pixels[(yTile * 8 + y) * graphic.CharacterData.Width + (xTile * 8 + x)];
                    }
                }
                
                graphicTiles.Add(tilePixels);
            }
        }
        
        var screenPixels = new IPixel[screen.ScreenData.Width * screen.ScreenData.Height];
        var tileIndex = 0;
        foreach (var tile in screen.ScreenData.Tiles)
        {
            var tileY = (tileIndex * 8 / screen.ScreenData.Width) * 8;
            var tileX = tileIndex * 8 % screen.ScreenData.Width;
            
            var tilePixels = graphicTiles[tile.TileIndex];
            for (var y = 0; y < 8; y++)
            {
                for (var x = 0; x < 8; x++)
                {
                    var pixel = tilePixels[(tile.FlipV ? 7 - y : y) * 8 + (tile.FlipU ? 7 - x : x)];
                    if (pixel is IndexedPixel indexedPixel)
                    {
                        indexedPixel.PaletteIndex = (byte) (paletteOverride ?? tile.PaletteIndex);
                    }
                    
                    screenPixels[(tileY + y) * screen.ScreenData.Width + (tileX + x)] = pixel;
                }
            }

            tileIndex++;
        }
        
        return new IndexedPaletteImage(
            $"{graphic.File!.NameWithoutExtension}",
            screenPixels,
            palette.PaletteData.Palettes,
            new ImageMetaData(screen.ScreenData.Width, screen.ScreenData.Height, 
                graphic.CharacterData.TextureFormat,
                IsFirstColorTransparent: firstColorIsTransparent)
        );
    }
}