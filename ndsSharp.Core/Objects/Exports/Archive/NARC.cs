using System.Diagnostics;
using ndsSharp.Core.Data;
using ndsSharp.Core.Extensions;
using ndsSharp.Core.Objects.Exports.Archive.Blocks;
using ndsSharp.Core.Objects.Files;
using ndsSharp.Core.Providers;
using Serilog;

namespace ndsSharp.Core.Objects.Exports.Archive;

public class NARC : NdsObject, IFileProvider
{
    public Dictionary<string, RomFile> Files { get; set; } = [];
    
    public override string Magic => "NARC";

    [Block] public FATB AllocationTable;
    [Block] public FNTB NameTable;
    [Block] public FIMG Image;
    public override void Deserialize(DataReader reader)
    {
        base.Deserialize(reader);
        
        for (ushort id = 0; id < AllocationTable.Pointers.Count; id++)
        {
            var pointer = AllocationTable.Pointers[id];
            if (pointer.Length <= 0) continue;

            string fileName;
            if (id >= NameTable.FirstId && id < NameTable.FilesById.Count)
            {
                fileName = NameTable.FilesById[id];
            }
            else
            {
                fileName = id.ToString();
            }
            
            var compression = Compression.GetCompression(Image.Reader, pointer);
            if (!fileName.Contains('.')) // detect extension
            {
                var readExtension = Image.Reader.PeekString(4).TrimEnd('0').ToLower();
                if (FileTypeRegistry.TryGetExtension(readExtension, out var realExtension))
                {
                    fileName += $".{realExtension}";
                }
                else
                {
                    fileName += ".bin";
                }
            }

            Files[fileName] = new RomFile(fileName, pointer)
            {
                Compression = compression
            };
        }
    }

    public T LoadObject<T>(string path) where T : BaseDeserializable, new() => LoadObject<T>(Files[path]);
    
    public T LoadObject<T>(RomFile file) where T : BaseDeserializable, new() => CreateReader(file).ReadObject<T>();
    
    public bool TryLoadObject<T>(string path, out T data) where T : BaseDeserializable, new() => TryLoadObject(Files[path], out data);
    
    public bool TryLoadObject<T>(RomFile file, out T data) where T : BaseDeserializable, new()
    {
        data = null!;
        try
        {
            data = LoadObject<T>(file);
            return true;
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
            return false;
        }
    }

    public DataReader CreateReader(RomFile file)
    {
        return Image.Reader.LoadPointer(file.Pointer);
    }
    
    public DataReader CreateReader(string path) => CreateReader(Files[path]);
}