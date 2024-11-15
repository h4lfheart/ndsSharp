using ndsSharp.Core.Data;
using ndsSharp.Core.Objects;
using ndsSharp.Core.Objects.Exports;
using ndsSharp.Core.Objects.Exports.Archive;
using ndsSharp.Core.Objects.Files;
using ndsSharp.Core.Objects.Rom;
using Serilog;

namespace ndsSharp.Core.Providers;

public class NdsFileProvider : IFileProvider
{
    public Dictionary<string, RomFile> Files { get; set; } = [];
    
    public RomHeader Header;
    public RomBanner Banner;

    public bool UnpackNARCFiles = false;

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

        _reader.Position = (int) Header.BannerOffset;
        Banner = new RomBanner(_reader);

        _allocationTable = new AllocationTable(_reader.LoadPointer(Header.FatPointer));
        _nameTable = new NameTable(_reader.LoadPointer(Header.FntPointer));
        
        Mount(_allocationTable, _nameTable);

        if (UnpackNARCFiles)
        {
            var narcFiles = GetAllFilesOfType<NARC>().ToArray();
            foreach (var narcFile in narcFiles)
            {
                var narc = LoadObject<NARC>(narcFile);
                var basePath = narcFile.Path.Replace(".narc", string.Empty);
                foreach (var (path, file) in narc.Files)
                {
                    var newPath = basePath + $"/{path}";
                    Files[newPath] = new RomFile(newPath, file.Pointer.GlobalFrom(narc.Image.Reader));
                }
                
                Files.Remove(narcFile.Path);
            }
        }
    }

    protected void Mount(AllocationTable allocationTable, NameTable nameTable)
    {
        for (ushort id = 0; id < allocationTable.Pointers.Count; id++)
        {
            var pointer = allocationTable.Pointers[id];
            if (pointer.Length <= 0) continue;
            
            if (id >= nameTable.FirstId)
            {
                var fileName = nameTable.FilesById[id];
                if (!fileName.Contains('.')) // detect extension
                {
                    var extension = _reader.PeekString(4, pointer.Offset).ToLower();
                    if (FileTypeRegistry.Contains(extension))
                    {
                        fileName += $".{extension}";
                    }
                    else
                    {
                        fileName += ".bin";
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
    
    public IEnumerable<RomFile> GetAllFilesOfType<T>() where T : NdsObject, new()
    {
        var accessor = new T();
        return Files.Values.Where(file => file.Type.Equals(accessor.Magic, StringComparison.OrdinalIgnoreCase));
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

    public BaseReader CreateReader(RomFile file)
    {
        return _reader.LoadPointer(file.Pointer);
    }
    
    public BaseReader CreateReader(string path) => CreateReader(Files[path]);

    public void LogFileStats()
    {
        var fileBreakdown = new Dictionary<string, int>();
        foreach (var (path, file) in Files)
        {
            fileBreakdown.TryAdd(file.Type, 0);
            fileBreakdown[file.Type]++;
        }

        fileBreakdown = fileBreakdown.OrderByDescending(x => x.Value).ToDictionary();

        Log.Information("Total Files: {Count}", Files.Count);
        foreach (var (type, count) in fileBreakdown)
        {
            Log.Information("{Type}: {Count}", type, count);
        }
    }
}