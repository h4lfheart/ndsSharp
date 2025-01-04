using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using GenericReader;
using ndsSharp.Core.Extensions;

namespace ndsSharp.Core.Data;

public class BaseReader : GenericBufferReader
{
    public BaseReader? Owner;
    public int AbsoluteOffset;
    public int AbsolutePosition => AbsoluteOffset + Position;
    
    public virtual BaseReader AbsoluteOwner
    {
        get
        {
            var cur = this;
            while (cur.Owner is not null)
            {
                cur = cur.Owner;
            }

            return cur;
        }
    }

    public int PositionZeroOffset;

    public new int Position
    {
        get => base.Position - PositionZeroOffset;
        set => base.Position = PositionZeroOffset + value;
    }

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
    
    public unsafe object Read(Type type)
    {
        var size = Marshal.SizeOf(type);
        var memory = ReadMemory(size);
        var memoryHandle = memory.Pin();

        var result = Marshal.PtrToStructure(new IntPtr(memoryHandle.Pointer), type)!;
        Position += size;
        return result;
    }
    
    public BaseReader Spliced(int? position = null, int? length = null)
    {
        Position = position ?? Position;
        length ??= Length - Position;
        return new BaseReader(ReadArray<byte>((int) length));
    }
    
    public void ReadWithZeroedPosition(Action<BaseReader> readFunc)
    {
        PositionZeroOffset = Position;
        readFunc.Invoke(this);
        PositionZeroOffset = 0;
        Position = base.Position;
    }
    
    public void ReadWithZeroedPosition(Action<BaseReader> readFunc, int offset)
    {
        Position = offset;
        ReadWithZeroedPosition(readFunc);
    }
    
    public void ReadWithZeroedPosition(Action readFunc)
    {
        ReadWithZeroedPosition(_ => readFunc());
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
    
    public string ReadNullTerminatedString(bool unicode = false)
    {
        var originalPos = Position;
        var length = 0;
        byte currentByte;
        do
        {
            currentByte = Read<byte>();
            length++;
        } while (currentByte != 0x00);

        Position = originalPos;
        return ReadString(length, unicode);
    }
    
    public T ReadEnum<T>() where T : Enum
    {
        var enumType = typeof(T);
        var primitiveType = enumType.GetEnumUnderlyingType();
        return (T) Read(primitiveType);
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
    
    
    public float ReadIntAsFloat() => Read<int>().ToFloat(1, 19, 12);
    public float ReadShortAsFloat() => FloatExtensions.ToFloat(Read<ushort>(), 1, 3, 12);
}