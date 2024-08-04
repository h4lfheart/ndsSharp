using ndsSharp.Core.Data;

namespace ndsSharp.Core.Objects.Files;

public class NameTable
{
    public ushort FirstId;
    public Dictionary<ushort, string> FilesById = new();

    private int StartOffset;

    private const int RootID = 0xF000;

    public NameTable(BaseReader reader)
    {
        StartOffset = reader.Position;
        FirstId = LoadDirectory(reader);
    }
    
    protected ushort LoadDirectory(BaseReader reader, int folderId = RootID, string folderName = "", string pathAtThisPoint = "")
    {
        pathAtThisPoint = string.IsNullOrEmpty(folderName) ? pathAtThisPoint : pathAtThisPoint + $"{folderName}/";
        
        reader.Position = (folderId & 0xFF) * 8 + StartOffset;
        
        var entryOffset = reader.Read<uint>();
        var fileId = reader.Read<ushort>();
        var parentId = reader.Read<ushort>();

        reader.Position = (int) entryOffset + StartOffset;

        var currentId = fileId;
        while (true)
        {
            var controlByte = reader.Read<byte>();
            if (controlByte == 0) break;

            var nameLength = controlByte & 0x7F;
            var name = reader.ReadString(nameLength);
            
            var isFile = (controlByte & 0x80) == 0;
            if (isFile)
            {
                FilesById[currentId] = pathAtThisPoint + name;
                currentId++;
            }
            else
            {
                var subFolderId = reader.Read<ushort>();
                reader.Peek(() => LoadDirectory(reader, subFolderId, name, pathAtThisPoint));
            }
        }

        return fileId;
    }
}