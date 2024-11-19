using ndsSharp.Core.Data;
using ndsSharp.Core.Objects.Exports;

namespace ndsSharp.Core.Objects.Files;

public class RomFile
{
    public string Path;
    public string Name => Path.Split("/").Last();
    public string Type => Path.Split(".").Last();

    public RomFile? Owner;

    public readonly DataPointer Pointer;

    public RomFile(string path, DataPointer pointer, RomFile? owner = null)
    {
        Path = path;
        Pointer = pointer;
        Owner = owner;
    }
}