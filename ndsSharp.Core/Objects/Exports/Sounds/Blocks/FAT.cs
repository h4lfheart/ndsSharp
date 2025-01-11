using ndsSharp.Core.Data;

namespace ndsSharp.Core.Objects.Exports.Sounds.Blocks;

public class FAT : NdsBlock
{
    public List<DataPointer> Pointers = [];
    
    public override string Magic => "FAT";

    public override void Deserialize(DataReader reader)
    {
        base.Deserialize(reader);
        
        var count = reader.Read<uint>();
        for (var i = 0; i < count; i++)
        {
            Pointers.Add(new DataPointer(reader));
            reader.Position += sizeof(uint) * 2; // reserved
        }
    }
}