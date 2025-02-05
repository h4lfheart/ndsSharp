using System.Diagnostics;
using System.Numerics;
using ndsSharp.Core.Data;
using ndsSharp.Core.Mathematics;
using ndsSharp.Core.Objects;
using ndsSharp.Core.Objects.Exports.Meshes;
using ndsSharp.Core.Plugins.BW2.Map.Data;

namespace ndsSharp.Core.Plugins.BW2.Map;

public class BW2Map : BW2Object
{
    public BMD Model;
    public BW2MapActor[] Actors = [];
    public BW2PermissionsData Permissions;
    public BW2TerrainData Terrain;

    private int ModelOffset = -1;
    private int PermissionsOffset = -1;
    private int ActorsOffset = -1;
    private int TerrainOffset = -1;
    
    public override void Deserialize(DataReader reader)
    {
        base.Deserialize(reader);

        ModelOffset = FileOffsets[0];
        switch (Magic)
        {
            case "NG":
            {
                ActorsOffset = FileOffsets[1];
                TerrainOffset = FileOffsets[2];
                break;
            }
            case "WB":
            case "DR":
            {
                PermissionsOffset = FileOffsets[1];
                ActorsOffset = FileOffsets[2];
                break;
            }
            case "GC":
            { 
                PermissionsOffset = FileOffsets[1];
                TerrainOffset = FileOffsets[2];
                ActorsOffset = FileOffsets[3];
                break;
            }
        }

        if (ModelOffset != -1)
        {
            reader.Position = ModelOffset;
            Model = reader.ReadObject<BMD>(zeroPosition: true);
        }

        if (PermissionsOffset != -1)
        {
            reader.Position = PermissionsOffset;
            Permissions = reader.ReadObject<BW2PermissionsData>();
        }

        if (ActorsOffset != -1)
        {
            reader.Position = ActorsOffset;
            Actors = reader.ReadArray(() => reader.ReadObject<BW2MapActor>());
        }

        if (TerrainOffset != -1)
        {
            reader.Position = TerrainOffset;
            Terrain = reader.ReadObject<BW2TerrainData>();
        }
    }
}
