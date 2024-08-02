using ndsSharp.Core.Data;
using ndsSharp.Core.Objects.Exports;
using ndsSharp.Core.Objects.Files;
using ndsSharp.Core.Objects.Rom;

namespace ndsSharp.Core;

public class NdsFileProvider
{
    public Dictionary<string, RomFile> Files = new();
    
    public RomHeader Header;

    private AllocationTable _allocationTable;
    private NameTable _nameTable;
    
    private BaseReader _reader;
    
    public NdsFileProvider(FileInfo romFile) : this(romFile.FullName)
    {
    }

    public NdsFileProvider(string filePath)
    {
        _reader = new BaseReader(File.ReadAllBytes(filePath));
    }

    public void Initialize()
    {
        Header = new RomHeader(_reader);

        _allocationTable = new AllocationTable(_reader.LoadPointer(Header.FatPointer));
        _nameTable = new NameTable(_reader.LoadPointer(Header.FntPointer));
        
        Mount(_allocationTable, _nameTable);
    }

    protected void Mount(AllocationTable allocationTable, NameTable nameTable)
    {
        for (ushort id = 0; id < allocationTable.Pointers.Count; id++)
        {
            var pointer = allocationTable.Pointers[id];
            if (pointer.Length <= 0) continue;
            
            if (id >= nameTable.FirstID)
            {
                var fileName = nameTable.FilesById[id];
                if (!fileName.Contains('.')) // detect extension
                {
                    var extension = _reader.PeekString(4, pointer.Offset).ToLower();
                    if (FileTypeRegistry.Contains(extension))
                    {
                        fileName += $".{extension}";
                    }
                }

                Files[fileName] = new RomFile(fileName, pointer);
            }
            else
            {
                var fileName = $"overlays/{id}.bin";
                Files[fileName] = new RomFile(fileName, pointer);
            }
        }
    }
}