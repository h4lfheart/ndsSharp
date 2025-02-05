using System.Numerics;
using ndsSharp.Core.Conversion.Models.Mesh;
using ndsSharp.Core.Plugins.BW2.Map.Data;

namespace ndsSharp.Core.Plugins.BW2.Extensions;

public static class BW2ModelExtensions
{
    private static readonly Vector2[] Vertices =
    [
        new(0, 0),
        new(1, 0),
        new(1, 1),
        new(0, 1),
    ];
    
    public static Model ExtractTerrainModel(this BW2TerrainData terrainData)
    {
        var model = new Model { Name = "Terrain" };
        var section = new Section { Name = "Terrain" };
        
        var vertexIndex = 0;
        foreach (var tile in terrainData.Tiles)
        {
            var heights = tile.HeightType == BW2HeightType.Default
                ? GetPlaneHeights(tile)
                : GetPlaneHeights(tile, terrainData.Corners[tile.Height]);
            
            section.Polygons.Add(new Polygon
            {
                PolygonType = PolygonType.QUAD,
                Vertices = Vertices.Select((vertex, index) => new Vector3(vertex.X + tile.X, heights[index], vertex.Y + tile.Y)).ToList()
            });
            
            section.Faces.Add(new Face("Terrain_Material")
            {
                VertexIndices = [vertexIndex + 0, vertexIndex + 1, vertexIndex + 3]
            });
            
            section.Faces.Add(new Face("Terrain_Material")
            {
                VertexIndices = [vertexIndex + 3, vertexIndex + 2, vertexIndex + 1]
            });

            vertexIndex += 4;
        }
        
        model.Sections.Add(section);
        return model;
    }

    private static float[] GetPlaneHeights(BW2TerrainTile tile)
    {
        return GetPlaneHeights(tile.X, tile.Y, BW2TerrainTable.GetSlopeX(tile.Slope),
            BW2TerrainTable.GetSlopeZ(tile.Slope), BW2TerrainTable.GetHeight(tile.Height));
    }

    private static float[] GetPlaneHeights(BW2TerrainTile tile, BW2TerrainCorner corner)
    {
        var plane1 = GetPlaneHeights(tile.X, tile.Y, BW2TerrainTable.GetSlopeX(corner.Slope1),
            BW2TerrainTable.GetSlopeZ(corner.Slope1), BW2TerrainTable.GetHeight(corner.Height1));
        var plane2 = GetPlaneHeights(tile.X, tile.Y, BW2TerrainTable.GetSlopeX(corner.Slope2),
            BW2TerrainTable.GetSlopeZ(corner.Slope2), BW2TerrainTable.GetHeight(corner.Height2));
        return [plane1[0], plane1[1], plane2[2], plane2[3]];
    }

    private static float[] GetPlaneHeights(int x, int y, float slopeX, float slopeZ, float height)
    {
        var heights = new float[4];
        var heightOffset = MathF.Sqrt(slopeX * slopeX + slopeZ * slopeZ + 1) * height;
        for (var vertexIndex = 0; vertexIndex < heights.Length; vertexIndex++)
        {
            var vertex = Vertices[vertexIndex];
            heights[vertexIndex] = slopeX * (x - 16 + vertex.X) + slopeZ * (y - 16 + vertex.Y) + heightOffset;
        }

        return heights;
    }
}