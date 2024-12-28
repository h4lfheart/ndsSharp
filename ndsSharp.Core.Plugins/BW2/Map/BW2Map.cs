using ndsSharp.Core.Data;
using ndsSharp.Core.Objects;
using ndsSharp.Core.Objects.Exports.Meshes;

namespace ndsSharp.Plugins.BW2.Map;

public class BW2Map : BW2Object
{
    public BMD Model;
    public List<BW2MapActor> Actors = [];
    
    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);
        
        ReadModel(reader);
        ReadActors(reader);
    }

    public void ReadModel(BaseReader reader)
    {
        var modelOffset = FileOffsets[0];
        
        reader.Position = (int) modelOffset;
        Model = reader.ReadObject<BMD>(zeroPosition: true);
    }

    public void ReadActors(BaseReader reader)
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