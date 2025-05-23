using System.Numerics;
using ndsSharp.Core.Data;
using ndsSharp.Core.Objects;
using Newtonsoft.Json;
using Serilog;

namespace ndsSharp.Core.Plugins.BW2.Map.Data;

public class BW2TerrainData : BaseDeserializable
{
    public List<BW2TerrainTile> Tiles = [];
    public BW2TerrainCorner[] Corners = [];
    
    public override void Deserialize(DataReader reader)
    {
        var width = reader.Read<ushort>();
        var height = reader.Read<ushort>();

        var maximumCornerIndex = -1;
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var tile = reader.ReadObject<BW2TerrainTile>(dataModifier: tile =>
                {
                    tile.X = x;
                    tile.Y = y;
                });
                
                if (tile.HeightType == BW2HeightType.Corner)
                {
                    maximumCornerIndex = Math.Max(maximumCornerIndex, tile.Height);
                }
                
                Tiles.Add(tile);
            }
        }

        Corners = reader.ReadArray(maximumCornerIndex + 1, () => reader.ReadObject<BW2TerrainCorner>());
    }
}

public class BW2TerrainTile : BaseDeserializable
{
    public BW2HeightType HeightType;
    public int Slope;
    public ushort Height;

    public int X;
    public int Y;
    
    public override void Deserialize(DataReader reader)
    {
        var slopeFlags = reader.Read<ushort>();
        HeightType = (BW2HeightType) (slopeFlags & 3);
        Slope = (ushort)(slopeFlags >> 2);
        Height = reader.Read<ushort>();
        reader.Position += sizeof(uint);
    }

    public override bool Equals(object? obj)
    {
        var other = (BW2TerrainTile)obj;
        return HeightType == other.HeightType && Slope == other.Slope && Height == other.Height && X == other.X &&
               Y == other.Y;
    }
}

public class BW2TerrainCorner : BaseDeserializable
{
    public ushort Slope1;
    public BW2HeightType HeightType;
    public ushort Slope2;
    public ushort Height1;
    public ushort Height2;
    
    public override void Deserialize(DataReader reader)
    {
        var slopeFlags = reader.Read<ushort>();
        HeightType = (BW2HeightType) (slopeFlags & 3);
        Slope1 = (ushort)(slopeFlags >> 2);
        Slope2 = reader.Read<ushort>();
        Height1 = reader.Read<ushort>();
        Height2 = reader.Read<ushort>();
    }
}

public enum BW2HeightType
{
    Default = 0,
    Corner = 2
}