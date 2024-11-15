using ndsSharp.Core.Data;
using ndsSharp.Core.Objects.Exports.Meshes.MDL;

namespace ndsSharp.Core.Objects.Exports.Meshes;

public class MDL0 : NdsBlock
{

    public List<MDL0Model> Models = [];
    
    public override string Magic => "MDL0";

    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);

        var modelList = reader.ReadNameListPrimitive<uint>();
        foreach (var (name, offset) in modelList.Items)
        {
            reader.Position = (int) offset;

            var model = reader.ReadObject<MDL0Model>(dataModifier: model => model.Name = name, zeroPosition: true);
            Models.Add(model);
        }
    }
}