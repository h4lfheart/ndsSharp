using AuroraLib.Compression.Interfaces;
using ndsSharp.Core.Data;
using ndsSharp.Core.Extensions;
using ndsSharp.Core.Objects.Exports;

namespace ndsSharp.Core.Objects.Files;

public class RomFile
{
    public string Path;
    public string Name => Path.SubstringAfterLast("/");
    public string Extension => Path.SubstringAfterLast(".");
    public string NameWithoutExtension => Path.SubstringAfterLast("/").SubstringBeforeLast(".");
    
    public Type? FileType => FileTypeRegistry.GetTypeOrDefault(Extension);

    public ICompressionAlgorithm? Compression;

    public RomFile? Owner;

    public readonly DataPointer Pointer;

    public RomFile(string path, DataPointer pointer, RomFile? owner = null)
    {
        Path = path;
        Pointer = pointer;
        Owner = owner;
    }
}