using ndsSharp.Core.Data;
using ndsSharp.Core.Objects.Files;

namespace ndsSharp.Core.Objects.Exports.Archive;

public class FATB : NdsBlock
{
    public List<DataPointer> Pointers = [];
    
    public override string Magic => "FATB";

    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);

        var fileCount = reader.Read<int>();
        var allocationTable = new AllocationTable(reader, fileCount);
        Pointers = allocationTable.Pointers;
    }
}