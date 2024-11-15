using ndsSharp.Core.Data;

namespace ndsSharp.Core.Objects;

public abstract class BaseDeserializable
{
    public abstract void Deserialize(BaseReader reader);
}

public abstract class DeserializableWithName : BaseDeserializable
{
    public string Name;
}

public static class DeserializableExtensions
{
    public static T ReadObject<T>(this BaseReader reader, bool zeroPosition = false) where T : BaseDeserializable
    {
        return ReadObject<T>(reader, typeof(T), zeroPosition);
    }
     
    public static T ReadObject<T>(this BaseReader reader, Action<T> dataModifier, bool zeroPosition = false) where T : BaseDeserializable
    {
        return ReadObject<T>(reader, typeof(T), dataModifier, zeroPosition);
    }
     
    public static T ReadObject<T>(this BaseReader reader, Type type, bool zeroPosition = false) where T : BaseDeserializable
    {
        var ret = Activator.CreateInstance(type) as T;
        ret!.Deserialize(zeroPosition ? reader.Spliced() : reader);
        return ret;
    }
     
    public static T ReadObject<T>(this BaseReader reader, Type type, Action<T> dataModifier, bool zeroPosition = false) where T : BaseDeserializable
    {
        var ret = Activator.CreateInstance(type) as T;
        dataModifier.Invoke(ret);
        ret!.Deserialize(zeroPosition ? reader.Spliced() : reader);
        return ret;
    }
}