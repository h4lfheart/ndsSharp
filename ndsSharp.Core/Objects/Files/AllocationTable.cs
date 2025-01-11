using ndsSharp.Core.Data;

namespace ndsSharp.Core.Objects.Files;

public unsafe class AllocationTable
{
    public List<DataPointer> Pointers = [];

    public AllocationTable(DataReader reader, int fileCount = -1)
    {
        if (fileCount == -1)
            fileCount = reader.Length / (sizeof(uint) * 2);
        
        for (var fileIndex = 0; fileIndex < fileCount; fileIndex++)
        {
            Pointers.Add(new DataPointer(reader, DataPointerType.StartEnd));
        }
    }
}