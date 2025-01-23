using ndsSharp.Core.Data;
using ndsSharp.Core.Objects.Exports;
using ndsSharp.Core.Objects.Files;

namespace ndsSharp.Core.Objects;

public abstract class BaseDeserializable
{
    public RomFile? File;
    public abstract void Deserialize(DataReader reader);
}

public abstract class DeserializableWithName : BaseDeserializable
{
    public string Name;
}

public static class DeserializableExtensions
{
    public static T ReadObject<T>(this DataReader reader, bool zeroPosition = false) where T : BaseDeserializable
    {
        return ReadObject<T>(reader, typeof(T), zeroPosition);
    }
     
    public static T ReadObject<T>(this DataReader reader, Action<T> dataModifier, bool zeroPosition = false) where T : BaseDeserializable
    {
        return ReadObject<T>(reader, typeof(T), dataModifier, zeroPosition);
    }
    
    public static BaseDeserializable ReadObject(this DataReader reader, Type type, bool zeroPosition = false)
    {
        return ReadObject<BaseDeserializable>(reader, type, zeroPosition);
    }
     
    public static BaseDeserializable ReadObject(this DataReader reader, Type type, Action<BaseDeserializable> dataModifier, bool zeroPosition = false)
    {
        return ReadObject<BaseDeserializable>(reader, type, dataModifier, zeroPosition);
    }
     
    public static T ReadObject<T>(this DataReader reader, Type type, bool zeroPosition = false) where T : BaseDeserializable
    {
        var ret = Activator.CreateInstance(type) as T;
        ret!.Deserialize(zeroPosition ? reader.Spliced() : reader);
        return ret;
    }
     
    public static T ReadObject<T>(this DataReader reader, Type type, Action<T> dataModifier, bool zeroPosition = false) where T : BaseDeserializable
    {
        var ret = Activator.CreateInstance(type) as T;
        dataModifier.Invoke(ret);
        ret!.Deserialize(zeroPosition ? reader.Spliced() : reader);
        return ret;
    }
}