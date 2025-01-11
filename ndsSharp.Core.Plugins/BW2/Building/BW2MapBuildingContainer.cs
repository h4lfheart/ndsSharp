using ndsSharp.Core.Data;
using ndsSharp.Core.Objects;
using ndsSharp.Core.Objects.Exports.Meshes;

namespace ndsSharp.Plugins.BW2.Building;

public class BW2MapBuildingContainer : BW2Object
{
    public readonly List<BW2MapBuildingDefinition> Definitions = [];
    public readonly List<BMD> Models = [];
    
    public override void Deserialize(DataReader reader)
    {
        base.Deserialize(reader);
        
        var buildingCount = FileOffsets.Length / 2;
        for (var buildingDefinitionOffset = 0; buildingDefinitionOffset < buildingCount; buildingDefinitionOffset++)
        {
            reader.Position = (int) FileOffsets[buildingDefinitionOffset];
            Definitions.Add(reader.ReadObject<BW2MapBuildingDefinition>(zeroPosition: true));
        }
        
        for (var buildingModelOffset = buildingCount; buildingModelOffset < buildingCount * 2; buildingModelOffset++)
        {
            reader.Position = (int) FileOffsets[buildingModelOffset];
            Models.Add(reader.ReadObject<BMD>(zeroPosition: true));
        }
    }
}