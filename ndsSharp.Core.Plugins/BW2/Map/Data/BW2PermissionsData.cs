using ndsSharp.Core.Data;
using ndsSharp.Core.Mathematics;
using ndsSharp.Core.Objects;

namespace ndsSharp.Core.Plugins.BW2.Map.Data;

public class BW2PermissionsData : BaseDeserializable
{
    public Matrix<ushort> TerrainMatrix;
    public Matrix<ushort> HeightMatrix;
    public Matrix<byte> FlagsMatrix;
    public Matrix<byte> CollisionMatrix;
    public Matrix<byte> ShadowMatrix;
    
    public override void Deserialize(DataReader reader)
    {
        var width = reader.Read<ushort>();
        var height = reader.Read<ushort>();
        
        TerrainMatrix = new Matrix<ushort>(width, height);
        HeightMatrix = new Matrix<ushort>(width, height);
        FlagsMatrix = new Matrix<byte>(width, height);
        CollisionMatrix = new Matrix<byte>(width, height);
        ShadowMatrix = new Matrix<byte>(width, height);
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                TerrainMatrix[x, y] = reader.Read<ushort>();
                HeightMatrix[x, y] = reader.Read<ushort>();
                FlagsMatrix[x, y] = reader.Read<byte>();
                reader.Position += sizeof(byte);
                CollisionMatrix[x, y] = reader.Read<byte>();
                ShadowMatrix[x, y] = reader.Read<byte>();
            }
        }
    }
}