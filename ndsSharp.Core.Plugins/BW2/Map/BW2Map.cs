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
    public BW2PermissionsData ExtraPermissions;
    public BW2TerrainData Terrain;
    public BW2TerrainData ExtraTerrain;

    private int ModelOffset = -1;
    private int ActorsOffset = -1;
    private int PermissionsOffset = -1;
    private int ExtraPermissionsOffset = -1;
    
    public override void Deserialize(DataReader reader)
    {
        base.Deserialize(reader);

        ModelOffset = FileOffsets[0];
        switch (Magic)
        {
            case "NG":
            {
                ActorsOffset = FileOffsets[1];
                break;
            }
            case "WB":
            {
                PermissionsOffset = FileOffsets[1];
                ActorsOffset = FileOffsets[2];
                break;
            }
            case "DR":
            {
                PermissionsOffset = FileOffsets[1];
                ActorsOffset = FileOffsets[2];
                break;
            }
            case "GC":
            { 
                PermissionsOffset = FileOffsets[1];
                ExtraPermissionsOffset = FileOffsets[2];
                ActorsOffset = FileOffsets[3];
                break;
            }
        }

        if (ModelOffset != -1)
        {
            reader.Position = ModelOffset;
            Model = reader.ReadObject<BMD>(zeroPosition: true);
        }
        
        if (ActorsOffset != -1)
        {
            reader.Position = ActorsOffset;
            Actors = reader.ReadArray(() => reader.ReadObject<BW2MapActor>());
        }

        if (PermissionsOffset != -1)
        {
            reader.Position = PermissionsOffset;
            Permissions = reader.ReadObject<BW2PermissionsData>();
            
            reader.Position = PermissionsOffset;
            Terrain = reader.ReadObject<BW2TerrainData>();
        }
        
        if (ExtraPermissionsOffset != -1)
        {
            reader.Position = ExtraPermissionsOffset;
            ExtraPermissions = reader.ReadObject<BW2PermissionsData>();

            reader.Position = ExtraPermissionsOffset;
            ExtraTerrain =reader.ReadObject<BW2TerrainData>();
        }
        
    }
}
