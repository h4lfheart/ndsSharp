using System.Runtime.InteropServices;

namespace ndsSharp.Core.Data;

public class DataPointer
{
    public int Offset;
    public int Length;
    public BaseReader? Owner;
    
    public DataPointer(BaseReader reader, DataPointerType pointerType = DataPointerType.OffsetLength)
    {
        Owner = reader;
        switch (pointerType)
        {
            case DataPointerType.OffsetLength:
            {
                Offset = reader.Read<int>();
                Length = reader.Read<int>();
                break;
            }
            case DataPointerType.StartEnd:
            {
                Offset = reader.Read<int>();
                
                var endOffset = reader.Read<int>();
                Length = endOffset - Offset;
                break;
            }
        }
    }

    public DataPointer(int offset, int length, BaseReader? owner = null)
    {
        Offset = offset;
        Length = length;

        Owner = owner;
    }

    public BaseReader Load()
    {
        return Owner?.LoadPointer(this) ?? throw new ParserException("Pointer does not have an owner to load from.");
    }

    public DataPointer GlobalFrom(BaseReader reader)
    {
        return new DataPointer(Offset + reader.AbsoluteOffset, Length);
    }
}

public enum DataPointerType
{
    OffsetLength,
    StartEnd
}