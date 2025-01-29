using System.Numerics;
using ndsSharp.Core.Data;
using ndsSharp.Core.Objects;
using ndsSharp.Core.Objects.Exports;

namespace ndsSharp.Core.Plugins.BW2.Building;

public class BW2MapBuildingDefinition : BaseDeserializable
{
    public ushort ID;
    public ushort BuildingType;
    
    public ushort DoorID;
    public Vector3 DoorLocation;

    public List<NdsObject> SubFiles = [];
    
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
        reader.ReadWithZeroedPosition(() =>
        {
            var subFileOffsets = reader.ReadArray<int>(subFileCount);

            reader.Position += sizeof(uint) * (4 - subFileCount); // max subFile count is 4, skip extra unused offsets

            foreach (var subFileOffset in subFileOffsets)
            {
                reader.Position = subFileOffset;
                
                var extension = reader.PeekString(4).TrimEnd('0').ToLower();
                if (FileTypeRegistry.TryGetType(extension, out var fileType))
                {
                    SubFiles.Add((NdsObject) reader.ReadObject(fileType, zeroPosition: true));
                }
            }
        });
    }
}