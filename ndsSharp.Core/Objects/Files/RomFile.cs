using ndsSharp.Core.Data;

namespace ndsSharp.Core.Objects.Files;

public class RomFile
{
    public string Path;
    public string Name => Path.Split("/").Last();

    public readonly DataPointer Pointer;

    public RomFile(string path, DataPointer pointer)
    {
        Path = path;
        Pointer = pointer;
    }
}