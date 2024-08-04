using System.Text;
using GenericReader;
using ndsSharp.Core.Extensions;
using ndsSharp.Core.Objects;

namespace ndsSharp.Core.Data;

public class BaseReader : GenericBufferReader
{
    public BaseReader(byte[] buffer, int start, int length) : base(buffer, start, length)
    {
    }

    public BaseReader(byte[] buffer) : base(buffer)
    {
    }

    public BaseReader(ReadOnlyMemory<byte> memory) : base(memory)
    {
    }

    public BaseReader(Memory<byte> memory) : base(memory)
    {
    }

    public BaseReader LoadPointer(DataPointer pointer)
    {
        return Peek(() =>
        {
            Position = pointer.Offset;
            var memory = ReadMemory(pointer.Length);
            return new BaseReader(memory);
        });
    }

    public string ReadString(int length, bool flip = false)
    {
        var str = ReadString(length, Encoding.UTF8);
        if (flip) str = str.Flip();
        return str;
    }
    
    // todo infer primitive type from underlying type
    public TEnumType ReadEnum<TEnumType, TPrimitiveType>() where TEnumType : Enum where TPrimitiveType : unmanaged
    {
        return (TEnumType) (object) Read<TPrimitiveType>();
    }
    
    public T Peek<T>(Func<T> func)
    {
        var originalPos = Position;
        var ret = func();
        Position = originalPos;
        return ret;
    }
    
    public void Peek(Action func)
    {
        var originalPos = Position;
        func();
        Position = originalPos;
    }

    public string PeekString(int length, int offset = -1)
    {
        if (offset >= 0)
            Position = offset;
        
        return Peek(() => ReadString(length));
    }
    
    public T ReadObject<T>() where T : BaseDeserializable
    {
        return ReadObject<T>(typeof(T));
    }
     
    public T ReadObject<T>(Action<T> dataModifier) where T : BaseDeserializable
    {
        return ReadObject<T>(typeof(T), dataModifier);
    }
     
    public T ReadObject<T>(Type type) where T : BaseDeserializable
    {
        var ret = Activator.CreateInstance(type) as T;
        ret!.Deserialize(this);
        return ret;
    }
     
    public T ReadObject<T>(Type type, Action<T> dataModifier) where T : BaseDeserializable
    {
        var ret = Activator.CreateInstance(type) as T;
        dataModifier.Invoke(ret);
        ret.Deserialize(this);
        return ret;
    }
}