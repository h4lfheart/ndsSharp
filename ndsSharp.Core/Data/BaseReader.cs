using System.Text;
using GenericReader;
using ndsSharp.Core.Extensions;
using ndsSharp.Core.Objects;
using ndsSharp.Core.Objects.Exports;

namespace ndsSharp.Core.Data;

public class BaseReader : GenericBufferReader
{
    public BaseReader? Owner;
    public int AbsoluteOffset;
    
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
            var pointerReader = new BaseReader(memory);
            pointerReader.Owner = this;
            pointerReader.AbsoluteOffset = AbsoluteOffset + Position;
            return pointerReader;
        });
    }

    public string ReadString(int length, bool flip = false, bool unicode = false)
    {
        var str = ReadString(length, unicode ? Encoding.Unicode : Encoding.UTF8);
        if (flip) str = str.Flip();
        return str.Trim('\0');
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
    
    public byte[] GetBuffer()
    {
        return Peek(() =>
        {
            Position = 0;
            return ReadArray<byte>(Length);
        });
    }
}