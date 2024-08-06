using ndsSharp.Core.Data;

namespace ndsSharp.Conversion.Textures.Colors;

public abstract class ColorDeserializer
{
    public abstract int Size { get; }

    public abstract Color Deserialize(BaseReader reader);
}


public abstract class ColorDeserializer<T> : ColorDeserializer where T : unmanaged
{
    public override unsafe int Size => sizeof(T);

    public override Color Deserialize(BaseReader reader)
    {
        var value = reader.Read<T>();
        return ProvideColor(value);
    }

    public abstract Color ProvideColor(T value);
}
