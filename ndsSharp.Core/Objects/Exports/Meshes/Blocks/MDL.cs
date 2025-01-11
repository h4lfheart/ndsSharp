using ndsSharp.Core.Data;
using ndsSharp.Core.Objects.Exports.Meshes.Model;

namespace ndsSharp.Core.Objects.Exports.Meshes.Blocks;

public class MDL : NdsBlock
{

    public List<MDLModel> Models = [];
    
    public override string Magic => "MDL";

    public override void Deserialize(DataReader reader)
    {
        base.Deserialize(reader);

        var modelList = reader.ReadNameListPrimitive<uint>();
        foreach (var (name, offset) in modelList.Items)
        {
            reader.Position = (int) offset;

            var model = reader.ReadObject<MDLModel>(dataModifier: model => model.Name = name, zeroPosition: true);
            Models.Add(model);
        }
    }
}