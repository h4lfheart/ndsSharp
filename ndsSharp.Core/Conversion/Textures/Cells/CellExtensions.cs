using ndsSharp.Core.Conversion.Textures.Images;
using ndsSharp.Core.Conversion.Textures.Images.Types;
using ndsSharp.Core.Conversion.Textures.Pixels;
using ndsSharp.Core.Extensions;
using ndsSharp.Core.Objects.Exports.Cells;
using ndsSharp.Core.Objects.Exports.Palettes;
using ndsSharp.Core.Objects.Exports.Textures;

namespace ndsSharp.Core.Conversion.Textures.Cells;

public static class CellExtensions
{
    public static List<IndexedPaletteImage> ExtractCells(this NCER cellData, NCGR graphic, NCLR palette, bool firstColorIsTransparent = true)
    {
        var pixels = graphic.CharacterData.Pixels;
        
        var xTiles = graphic.CharacterData.Width / 8;
        var yTiles = graphic.CharacterData.Height / 8;

        var pixelCells = new List<IPixel[]>();
        for (var yTile = 0; yTile < yTiles; yTile++)
        {
            for (var xTile = 0; xTile < xTiles; xTile++)
            {
                var tilePixels = new IPixel[64];
                for (var y = 0; y < 8; y++)
                {
                    for (var x = 0; x < 8; x++)
                    {
                        tilePixels[y * 8 + x] = pixels[(yTile * 8 + y) * graphic.CharacterData.Width + (xTile * 8 + x)];
                    }
                }
                
                pixelCells.Add(tilePixels);
            }
        }

        var outputImages = new List<IndexedPaletteImage>();
        for (var cellIndex = 0; cellIndex < cellData.CellData.Cells.Count; cellIndex++)
        {
            var cell = cellData.CellData.Cells[cellIndex];
            for (var objectIndex = 0; objectIndex < cell.ObjectCount; objectIndex++)
            {
                var cellObject = cellData.CellData.CellObjects[(int) (objectIndex + cell.ObjectAttributeIndex)];
                var objectWidth = cellObject.TileWidth * 8;
                var objectHeight = cellObject.TileHeight * 8;
                
                var tileCount = cellObject.TileWidth * cellObject.TileHeight;
                var cellPixels = pixelCells[cellObject.TileIndex..(cellObject.TileIndex + tileCount)];

                var objectPixels = new IPixel[objectWidth * objectHeight];
                for (var tileIndex = 0; tileIndex < tileCount; tileIndex++)
                {
                    var tilePixels = cellPixels[tileIndex];
                    
                    var tileX = tileIndex % cellObject.TileWidth;
                    var tileY = tileIndex / cellObject.TileWidth;
                    
                    for (var y = 0; y < 8; y++)
                    {
                        for (var x = 0; x < 8; x++)
                        {
                            objectPixels[(tileY * 8 + y) * objectWidth + (tileX * 8 + x)] = tilePixels[y * 8 + x];
                        }
                    }
                }

                var image = new IndexedPaletteImage(
                    $"{graphic.File!.NameWithoutExtension}_Cell_{cellIndex}_Object_{objectIndex}",
                    objectPixels.ToArray(),
                    palette.PaletteData.Palettes,
                    new ImageMetaData(objectWidth, objectHeight, graphic.CharacterData.TextureFormat,
                        FlipU: cellObject.FlipHorizontal,
                        FlipV: cellObject.FlipVertical,
                        IsFirstColorTransparent: firstColorIsTransparent)
                );
                
                outputImages.Add(image);
            }
        }
        
        return outputImages;
    }
}