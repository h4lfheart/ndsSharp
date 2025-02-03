using ndsSharp.Core.Data;
using ndsSharp.Core.Mathematics;
using ndsSharp.Core.Objects;
using ndsSharp.Core.Objects.Exports.Meshes;

namespace ndsSharp.Core.Plugins.BW2.Map;

public class BW2Map : BW2Object
{
    public BMD Model;
    
    public List<BW2MapActor> Actors = [];

    public Matrix<byte> TerrainMatrix = new(32, 32);
    public Matrix<byte> Terrain2Matrix = new(32, 32);
    public Matrix<byte> HeightMatrix = new(32, 32);
    public Matrix<byte> Height2Matrix = new(32, 32);
    public Matrix<byte> FlagsMatrix = new(32, 32);
    public Matrix<byte> CollisionMatrix = new(32, 32);
    public Matrix<byte> ShadowMatrix = new(32, 32);
    
    public override void Deserialize(DataReader reader)
    {
        base.Deserialize(reader);
        
        ReadModel(reader);
        ReadPermissions(reader);
        ReadActors(reader);
    }

    private void ReadModel(DataReader reader)
    {
        var modelOffset = FileOffsets[0];
        
        reader.Position = (int) modelOffset;
        Model = reader.ReadObject<BMD>(zeroPosition: true);
    }
    
    private void ReadPermissions(DataReader reader)
    {
        if (Magic == "NG") return;
        
        var permissionOffset = FileOffsets[1];
        
        reader.Position = (int) permissionOffset;

        reader.Position += 4; // what is this
        
        for (var y = 0; y < 32; y++)
        {
            for (var x = 0; x < 32; x++)
            {
                TerrainMatrix[x, y] = reader.Read<byte>();
                Terrain2Matrix[x, y] = reader.Read<byte>();
                HeightMatrix[x, y] = reader.Read<byte>();
                Height2Matrix[x, y] = reader.Read<byte>();
                FlagsMatrix[x, y] = reader.Read<byte>();
                reader.Position += sizeof(byte);
                CollisionMatrix[x, y] = reader.Read<byte>();
                ShadowMatrix[x, y] = reader.Read<byte>();
            }
        }
    }

    private void ReadActors(DataReader reader)
    {
        var actorsOffset = Magic switch
        {
            "NG" => FileOffsets[1],
            "WB" => FileOffsets[2],
            "DR" => FileOffsets[2],
            _ => FileOffsets[3]
        };

        reader.Position = (int) actorsOffset;

        var actorCount = reader.Read<uint>();
        for (var buildingIndex = 0; buildingIndex < actorCount; buildingIndex++)
        {
            Actors.Add(reader.ReadObject<BW2MapActor>());
        }
    }
}