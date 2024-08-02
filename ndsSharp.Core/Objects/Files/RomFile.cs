using ndsSharp.Core.Data;

namespace ndsSharp.Core.Objects.Files;

public class RomFile
{
    public string Path;
    public string Name => Path.Split("/").Last();

    private readonly DataPointer _pointer;

    public RomFile(string path, DataPointer pointer)
    {
        Path = path;
        _pointer = pointer;
    }
}