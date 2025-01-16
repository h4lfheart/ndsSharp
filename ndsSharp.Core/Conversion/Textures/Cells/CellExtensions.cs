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
    public static List<IndexedPaletteImage> ExtractCells(this NCER ncer, NCGR ncgr, NCLR nclr, bool firstColorIsTransparent = true)
    {
        var image = ncgr.Combine(nclr, firstColorIsTransparent);
        var cells = ncer.CellData.Cells;

        var outputImages = new List<IndexedPaletteImage>();
        var cellIndex = 0;
        foreach (var cell in cells)
        {
            var objectStartIndex = (int) cell.ObjectAttributeIndex;
            for (var objectIndex = 0; objectIndex < cell.ObjectCount; objectIndex++)
            {
                var cellObject = ncer.CellData.CellObjects[objectStartIndex  + objectIndex];

                var cellWidth = cellObject.TileWidth * 8;
                var cellHeight = cellObject.TileWidth * 8;
                
                var tilesPerRow = image.MetaData.Width / 8;

                var tileRow = cellObject.TileIndex / tilesPerRow;
                var tileCol = cellObject.TileIndex % tilesPerRow;

                var startX = tileCol * cellWidth;
                var startY = tileRow * cellHeight;
                
                var pixels = new List<IPixel>();
                for (var y = 0; y < cellHeight; y++)
                {
                    for (var x = 0; x < cellWidth; x++)
                    {
                        var pixelIndex = (startY + y) * image.MetaData.Width + (startX + x);
                        pixels.Add(image.Pixels[pixelIndex]);
                    }
                }

                var meta = new ImageMetaData(cellWidth, cellHeight, image.MetaData.Format, IsFirstColorTransparent: firstColorIsTransparent);
                outputImages.Add(new IndexedPaletteImage($"{image.Name}_Cell_{cellIndex}_Object_{objectIndex}", pixels.ToArray(), image.Palettes, meta));

            }

            cellIndex++;
        }

        return outputImages;
    }
}