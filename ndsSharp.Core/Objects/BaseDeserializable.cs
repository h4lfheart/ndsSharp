using ndsSharp.Core.Data;

namespace ndsSharp.Core.Objects;

public abstract class BaseDeserializable
{
    public abstract void Deserialize(BaseReader reader);
}

public static class DeserializableExtensions
{
    public static T ReadObject<T>(this BaseReader reader) where T : BaseDeserializable
    {
        return ReadObject<T>(reader, typeof(T));
    }
     
    public static T ReadObject<T>(this BaseReader reader, Action<T> dataModifier) where T : BaseDeserializable
    {
        return ReadObject<T>(reader, typeof(T), dataModifier);
    }
     
    public static T ReadObject<T>(this BaseReader reader, Type type) where T : BaseDeserializable
    {
        var ret = Activator.CreateInstance(type) as T;
        ret!.Deserialize(reader);
        return ret;
    }
     
    public static T ReadObject<T>(this BaseReader reader, Type type, Action<T> dataModifier) where T : BaseDeserializable
    {
        var ret = Activator.CreateInstance(type) as T;
        dataModifier.Invoke(ret);
        ret.Deserialize(reader);
        return ret;
    }
}