using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using ndsSharp.Core.Data.Reader;
using ndsSharp.Core.Extensions;

namespace ndsSharp.Core.Data;

public class DataReader : MemoryReader
{
    public DataReader? Owner;
    public int AbsoluteOffset;
    public int AbsolutePosition => AbsoluteOffset + Position;
    
    public virtual DataReader AbsoluteOwner
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

    public DataReader(byte[] buffer, int start, int length) : base(buffer, start, length)
    {
    }

    public DataReader(byte[] buffer) : base(buffer)
    {
    }

    public DataReader(ReadOnlyMemory<byte> memory) : base(memory)
    {
    }

    public DataReader(Memory<byte> memory) : base(memory)
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
    
    public DataReader Spliced(int? position = null, int? length = null)
    {
        Position = position ?? Position;
        length ??= Length - Position;
        return new DataReader(ReadArray<byte>((int) length));
    }
    
    public void ReadWithZeroedPosition(Action<DataReader> readFunc)
    {
        PositionZeroOffset = Position;
        readFunc.Invoke(this);
        PositionZeroOffset = 0;
        Position = base.Position;
    }
    
    public void ReadWithZeroedPosition(Action<DataReader> readFunc, int offset)
    {
        Position = offset;
        ReadWithZeroedPosition(readFunc);
    }
    
    public void ReadWithZeroedPosition(Action readFunc)
    {
        ReadWithZeroedPosition(_ => readFunc());
    }


    public DataReader LoadPointer(DataPointer pointer)
    {
        return Peek(() =>
        {
            Position = pointer.Offset;
            var memory = ReadMemory(pointer.Length);
            var pointerReader = new DataReader(memory);
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
    
    public TEnumType ReadEnum<TEnumType, TPrimitiveType>() where TEnumType : Enum where TPrimitiveType : struct
    {
        var numberValue = Read<TPrimitiveType>();
        var underlyingType = Enum.GetUnderlyingType(typeof(TEnumType));
        var convertedValue = Convert.ChangeType(numberValue, underlyingType);
        return (TEnumType) convertedValue;
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