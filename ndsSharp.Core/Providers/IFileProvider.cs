using ndsSharp.Core.Data;
using ndsSharp.Core.Objects;
using ndsSharp.Core.Objects.Files;
using Serilog;

namespace ndsSharp.Core.Providers;

public interface IFileProvider
{

    public Dictionary<string, RomFile> Files { get; set; }
    public T LoadObject<T>(string path) where T : BaseDeserializable, new();

    public T LoadObject<T>(RomFile file) where T : BaseDeserializable, new();

    public bool TryLoadObject<T>(string path, out T data) where T : BaseDeserializable, new();

    public bool TryLoadObject<T>(RomFile file, out T data) where T : BaseDeserializable, new();

    public DataReader CreateReader(RomFile file);

    public DataReader CreateReader(string path);
}