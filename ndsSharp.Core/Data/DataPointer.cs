using System.Runtime.InteropServices;

namespace ndsSharp.Core.Data;

public class DataPointer
{
    public int Offset;
    public int Length;
    public DataReader? Owner;
    
    public DataPointer(DataReader reader, DataPointerType pointerType = DataPointerType.OffsetLength)
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

    public DataPointer(int offset, int length, DataReader? owner = null)
    {
        Offset = offset;
        Length = length;

        Owner = owner;
    }

    public DataReader Load()
    {
        return Owner?.LoadPointer(this) ?? throw new ParserException("Pointer does not have an owner to load from.");
    }

    public DataPointer GlobalFrom(DataReader reader)
    {
        return new DataPointer(Offset + reader.AbsoluteOffset, Length);
    }
}

public enum DataPointerType
{
    OffsetLength,
    StartEnd
}