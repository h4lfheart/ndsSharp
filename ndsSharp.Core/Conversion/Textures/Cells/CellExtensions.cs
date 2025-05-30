using System.Diagnostics;
using ndsSharp.Core.Conversion.Textures.Images;
using ndsSharp.Core.Conversion.Textures.Images.Types;
using ndsSharp.Core.Conversion.Textures.Pixels;
using ndsSharp.Core.Conversion.Textures.Pixels.Indexed;
using ndsSharp.Core.Extensions;
using ndsSharp.Core.Objects.Exports.Cells;
using ndsSharp.Core.Objects.Exports.Palettes;
using ndsSharp.Core.Objects.Exports.Textures;

namespace ndsSharp.Core.Conversion.Textures.Cells;

public static class CellExtensions
{
    public static IndexedPaletteImage? ExtractCell(this NCER cellData, NCGR graphic, NCLR palette, int cellIndex, int? paletteOverride = null, bool firstColorIsTransparent = true)
    {
        var pixels = graphic.CharacterData.Pixels;
        var xTiles = graphic.CharacterData.TileWidth;
        var yTiles = graphic.CharacterData.TileHeight;

        var cell = cellData.CellData.Cells[cellIndex];
        if (cell.ObjectCount == 0) return null;
        
        var objectImages = new List<IPixel[]>();
        for (var objectIndex = 0; objectIndex < cell.ObjectCount; objectIndex++)
        {
            var cellObject = cellData.CellData.CellObjects[(int) (objectIndex + cell.ObjectAttributeIndex)];
            var objectWidth = cellObject.TileWidth * 8;
            var objectHeight = cellObject.TileHeight * 8;

            var tileCount = cellObject.TileWidth * cellObject.TileHeight;

            var objectPixels = new IPixel[objectWidth * objectHeight];
            for (var tileIndex = 0; tileIndex < tileCount; tileIndex++)
            {
                var absoluteTileIndex = cellObject.TileIndex + tileIndex;

                var tileX = tileIndex % cellObject.TileWidth;
                var tileY = tileIndex / cellObject.TileWidth;

                var absoluteTileX = absoluteTileIndex % xTiles;
                var absoluteTileY = absoluteTileIndex / xTiles;

                for (var y = 0; y < 8; y++)
                {
                    for (var x = 0; x < 8; x++)
                    {
                        var actualX = absoluteTileX * 8 + x;
                        var actualY = absoluteTileY * 8 + y;
        
                        var pixel = pixels[actualY * graphic.CharacterData.Width + actualX];
        
                        objectPixels[(tileY * 8 + y) * objectWidth + (tileX * 8 + x)] = pixel;
                    }
                }
            }
            
            objectImages.Add(objectPixels);
        }

        var combinedWidth = 0;
        var combinedHeight = 0;
        if (cell.UseBounds)
        {
            combinedWidth = cell.RightBound - cell.LeftBound + 1;
            combinedHeight = cell.BottomBound - cell.TopBound + 1;
        }
        else
        {
            for (var i = 0; i < cell.ObjectCount; i++)
            {
                var obj = cellData.CellData.CellObjects[(int)(i + cell.ObjectAttributeIndex)];
                var objRight = obj.X + obj.TileWidth * 8;
                var objBottom = obj.Y + obj.TileHeight * 8;
                if (objRight > combinedWidth) combinedWidth = objRight;
                if (objBottom > combinedHeight) combinedHeight = objBottom;
            }
        }
        
        var combinedPixels = new IPixel[combinedWidth * combinedHeight];
        var offsetX = cell.UseBounds ? -cell.LeftBound : 0;
        var offsetY = cell.UseBounds ? -cell.TopBound : 0;
        for (var objectIndex = 0; objectIndex < cell.ObjectCount; objectIndex++)
        {
            var cellObject = cellData.CellData.CellObjects[(int)(objectIndex + cell.ObjectAttributeIndex)];
            
            var objectPixels = objectImages[objectIndex];
            var objectWidth = cellObject.TileWidth * 8;
            var objectHeight = cellObject.TileHeight * 8;

            var xOffset = (short) cellObject.X;
            if (xOffset >= 0x100)
                xOffset -= 0x200;
            
            var yOffset = (short) cellObject.Y;
            if (yOffset >= 0x80)
                yOffset -= 0x100;
            
            for (var y = 0; y < objectHeight; y++)
            {
                for (var x = 0; x < objectWidth; x++)
                {
                    var srcX = cellObject.FlipHorizontal ? (objectWidth - 1 - x) : x;
                    var srcY = cellObject.FlipVertical ? (objectHeight - 1 - y) : y;

                    var destX = xOffset + x + offsetX;
                    var destY = yOffset + y + offsetY;

                    if (destX < 0 || destX >= combinedWidth || destY < 0 || destY >= combinedHeight) continue;
                    
                    var srcPixel = objectPixels[srcY * objectWidth + srcX];
                    var destPixel = combinedPixels[destY * combinedWidth + destX];

                    if (srcPixel is IndexedPixel srcIndexedPixel)
                    {
                        srcPixel = new IndexedPixel(srcIndexedPixel.Index, srcIndexedPixel.Alpha, (byte) (paletteOverride ?? cellObject.PaletteIndex));
                    }
                        
                    if (destPixel is null || (destPixel is IndexedPixel indexedPixel && (indexedPixel.Alpha == 0 || (indexedPixel.Index == 0 && firstColorIsTransparent))))
                        combinedPixels[destY * combinedWidth + destX] = srcPixel;
                }
            }
        }

        return new IndexedPaletteImage(
            $"Cell_{cellIndex}",
            combinedPixels,
            palette.PaletteData.Palettes,
            new ImageMetaData(
                combinedWidth,
                combinedHeight,
                graphic.CharacterData.TextureFormat,
                FlipU: false,
                FlipV: false,
                IsFirstColorTransparent: firstColorIsTransparent
            )
        );
    }

    public static List<IndexedPaletteImage> ExtractCells(this NCER cell, NCGR graphic, NCLR palette, bool firstColorIsTransparent = true)
    {
        var outputImages = new List<IndexedPaletteImage>();
        
        for (var cellIndex = 0; cellIndex < cell.CellData.Cells.Count; cellIndex++)
        {
            var cellImage = cell.ExtractCell(graphic, palette, cellIndex, firstColorIsTransparent: firstColorIsTransparent);
            if (cellImage is not null) outputImages.Add(cellImage);
        }
        
        return outputImages;
    }
}