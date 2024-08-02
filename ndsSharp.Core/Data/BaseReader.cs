using System.Text;
using GenericReader;

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

    public string ReadString(int length)
    {
        return ReadString(length, Encoding.UTF8);
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

    public string PeekString(int length, int offset)
    {
        Position = offset;
        return Peek(() => ReadString(4));
    }
}