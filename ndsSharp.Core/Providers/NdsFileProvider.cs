using System.Diagnostics;
using System.Reflection;
using AuroraLib.Compression.Algorithms;
using ndsSharp.Core.Data;
using ndsSharp.Core.Data.Reader;
using ndsSharp.Core.Extensions;
using ndsSharp.Core.Objects;
using ndsSharp.Core.Objects.Exports;
using ndsSharp.Core.Objects.Exports.Archive;
using ndsSharp.Core.Objects.Exports.Sounds;
using ndsSharp.Core.Objects.Files;
using ndsSharp.Core.Objects.Rom;
using ndsSharp.Core.Plugins;
using Serilog;

namespace ndsSharp.Core.Providers;

public class NdsFileProvider : IFileProvider
{
    public Dictionary<string, RomFile> Files { get; set; } = [];
    public Dictionary<Type, BasePlugin> Plugins { get; set; } = [];
    
    public RomHeader Header;
    public RomBanner Banner;

    public bool UnpackNARCFiles = false;
    public bool UnpackSDATFiles = false;

    private AllocationTable _allocationTable;
    private NameTable _nameTable;
    
    private DataReader _reader;
    
    public NdsFileProvider(FileInfo romFile) : this(romFile.FullName)
    {
    }

    public NdsFileProvider(string filePath)
    {
        _reader = new DataReader(File.ReadAllBytes(filePath));
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
                if (!TryLoadObject<NARC>(narcFile, out var narc))
                {
                    Log.Warning("Failed to mount NARC {Path}", narcFile.Path);
                    continue;
                }
                
                var basePath = narcFile.Path.Replace(".narc", string.Empty);
                foreach (var (path, file) in narc.Files)
                {
                    var newPath = basePath + $"/{path}";
                    Files[newPath] = new RomFile(newPath, file.Pointer.GlobalFrom(narc.Image.Reader), narcFile)
                    {
                        Compression = file.Compression
                    };
                }
                
                Files.Remove(narcFile.Path);
            }
        }

        if (UnpackSDATFiles)
        {
            var sdatFiles = GetAllFilesOfType<SDAT>().ToArray();
            foreach (var sdatFile in sdatFiles)
            {
                if (!TryLoadObject<SDAT>(sdatFile, out var sdat))
                {
                    Log.Warning("Failed to mount SDAT {Path}", sdatFile.Path);
                    continue;
                }
                
                var basePath = sdatFile.Path.Replace(".sdat", string.Empty);
                foreach (var soundType in Enum.GetValues<SoundFileType>())
                {
                    if (soundType is SoundFileType.GroupPlayer or SoundFileType.StreamPlayer or SoundFileType.Group) continue;
                    
                    var typeName = soundType.GetDescription();
                    var symbols = sdat.Symbols.Records[soundType];
                    var infos = sdat.Info.Records[soundType];

                    for (ushort index = 0; index < symbols.Count; index++)
                    {
                        var info = infos[index];
                        var data = sdat.FileAllocationTable.Pointers[info.FileID];
                        
                        var newPath = basePath + $"/{typeName}/{symbols[index]}.{typeName}".ToLower();
                        Files[newPath] = new SDATRomFile(newPath, data.GlobalFrom(sdat.Reader), info, index, sdatFile);
                    }
                }
                
                Files.Remove(sdatFile.Path);
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
                
                var compression = Compression.GetCompression(_reader, pointer);
                if (!fileName.Contains('.')) // detect extension
                {
                    var readExtension = _reader.PeekString(4).TrimEnd('0').ToLower();
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
            else
            {
                var fileName = $"overlays/{id}.bin";
                Files[fileName] = new RomFile(fileName, pointer);
            }
        }
    }
    
    public T? GetPluginInterface<T>() where T : BasePlugin
    {
        return (T?) GetPluginInterface(typeof(T));
    }
    
    public BasePlugin? GetPluginInterface(Type type)
    {
        return Plugins.GetValueOrDefault(type);
    }
    
    public IEnumerable<RomFile> GetAllFilesOfType<T>() where T : BaseDeserializable, new()
    {
        return Files.Values.Where(file => file.FileType == typeof(T));
    }
    
    public T LoadObject<T>(string path) where T : BaseDeserializable, new() => LoadObject<T>(Files[path]);
    
    public T LoadObject<T>(RomFile file) where T : BaseDeserializable, new() => (T) LoadObject(file, typeof(T));
    
    public BaseDeserializable LoadObject(string path, Type type) => LoadObject(Files[path], type);

    public BaseDeserializable LoadObject(RomFile file, Type type)
    {
        if (file.FileType is not null && file.FileType != type)
        {
            throw new ParserException($"Type mismatch for {file.Path}. Expected {file.FileType?.Name}, got {type.Name}");
        }

        var reader = CreateReader(file);
        if (file.Compression is not null)
        {
            var compressedStream = new MemoryReaderStream(reader);
            var uncompressedStream = new MemoryStream();
            file.Compression.Decompress(compressedStream, uncompressedStream);

            reader = new DataReader(uncompressedStream.GetBuffer());
        }
        
        return reader.ReadObject(type, dataModifier: obj => obj.File = file);
    }
    
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
        return _reader.LoadPointer(file.Pointer);
    }
    
    public DataReader CreateReader(string path) => CreateReader(Files[path]);
}