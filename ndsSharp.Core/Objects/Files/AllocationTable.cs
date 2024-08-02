using ndsSharp.Core.Data;

namespace ndsSharp.Core.Objects.Files;

public unsafe class AllocationTable
{
    public List<DataPointer> Pointers = [];

    public AllocationTable(BaseReader reader)
    {
        var fileCount = reader.Length / sizeof(DataPointer);
        for (var fileIndex = 0; fileIndex < fileCount; fileIndex++)
        {
            Pointers.Add(new DataPointer(reader, DataPointerType.StartEnd));
        }
    }
}