using System.Collections;
using ndsSharp.Core.Data;

namespace ndsSharp.Core.Objects.Exports;

public class NameList<T> : IEnumerable<KeyValuePair<string, T>>
{
    public string[] Names => Items.Keys.ToArray();
    public T[] Values => Items.Values.ToArray();
    public int Count => Items.Count;
    public T this[string name] => Items[name];
    public KeyValuePair<string, T> this[int index] => Items.ElementAt(index);
    
    
    public Dictionary<string, T> Items = [];

    public NameList(DataReader reader, Func<T> func)
    {
        var dummy = reader.Read<byte>();
        var count = reader.Read<byte>();
        var size = reader.Read<ushort>();

        var subheaderSize = reader.Read<ushort>();
        var unknownSize = reader.Read<ushort>();
        var unknown = reader.Read<uint>();
        var unknownArray = reader.ReadArray<uint>(count);

        var elementSize = reader.Read<ushort>();
        var dataSize = reader.Read<ushort>();

        var data = reader.ReadArray(count, func);
        var names = reader.ReadArray(count, () => reader.ReadString(16));

        for (var i = 0; i < count; i++)
        {
            Items[names[i]] = data[i];
        }
    }

    IEnumerator<KeyValuePair<string, T>> IEnumerable<KeyValuePair<string, T>>.GetEnumerator()
    {
        return Items.GetEnumerator();
    }

    public IEnumerator GetEnumerator()
    {
        return Items.GetEnumerator();
    }
}

public static class NameListExtensions
{
    public static NameList<T> ReadNameList<T>(this DataReader reader, Func<T> valueFunction)
    {
        return new NameList<T>(reader, valueFunction);
    }
    
    public static NameList<T> ReadNameList<T>(this DataReader reader) where T : BaseDeserializable
    {
        return new NameList<T>(reader, () => reader.ReadObject<T>());
    }
    
    public static NameList<T> ReadNameListPrimitive<T>(this DataReader reader) where T : unmanaged
    {
        return new NameList<T>(reader, reader.Read<T>);
    }
}