using System.Numerics;
using ndsSharp.Core.Data;
using ndsSharp.Core.Objects;

namespace ndsSharp.Core.Plugins.BW2.Building;

public class BW2MapBuildingDefinition : BaseDeserializable
{
    public ushort ID;
    public ushort BuildingType;
    
    public ushort DoorID;
    public Vector3 DoorLocation;
    
    public override void Deserialize(DataReader reader)
    {
        ID = reader.Read<ushort>();
        BuildingType = reader.Read<ushort>();
        
        DoorID = reader.Read<ushort>();
        DoorLocation.X = reader.Read<ushort>();
        DoorLocation.Y = reader.Read<ushort>();
        DoorLocation.Z = reader.Read<ushort>();

        reader.Position += 7;

        var subFileCount = reader.Read<byte>();
        var subFileOffsets = reader.ReadArray<uint>(subFileCount);
        
        // TODO actually read the sub files for anims n stuff
    }
}