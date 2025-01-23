using ndsSharp.Core.Data;
using ndsSharp.Core.Extensions;

namespace ndsSharp.Core.Objects.Exports.Cells.Blocks;

public class CEBK : NdsBlock
{
    public ushort CellCount;
    public bool UseBounds;
    public uint SizeBoundary;

    public List<Cell> Cells = [];
    public List<CellObject> CellObjects = [];
    
    public override string Magic => "CEBK";

    public override void Deserialize(DataReader reader)
    {
        base.Deserialize(reader);

        CellCount = reader.Read<ushort>();
        UseBounds = reader.Read<ushort>() == 1;

        var cellDataOffset = reader.Read<uint>();

        SizeBoundary = reader.Read<uint>() << 6;

        reader.Position += sizeof(uint) * 3;

        reader.Position = (int) cellDataOffset + HEADER_SIZE;
        for (var cellIndex = 0; cellIndex < CellCount; cellIndex++)
        {
            Cells.Add(reader.ReadObject<Cell>(dataModifier: cell => cell.UseBounds = UseBounds));
        }
        
        for (var cellIndex = 0; cellIndex < CellCount; cellIndex++)
        {
            var objectCount = Cells[cellIndex].ObjectCount;
            for (var objectIndex = 0; objectIndex < objectCount; objectIndex++)
            {
                CellObjects.Add(reader.ReadObject<CellObject>());
            }
        }
    }
}

public class Cell : BaseDeserializable
{
    public ushort ObjectCount;
    public uint ObjectAttributeOffset;
    public uint ObjectAttributeIndex => ObjectAttributeOffset / 6;
    
    public short RightBound;
    public short BottomBound;
    public short LeftBound;
    public short TopBound;

    public bool UseBounds;
    
    public override void Deserialize(DataReader reader)
    {
        ObjectCount = reader.Read<ushort>();

        reader.Position += sizeof(ushort);

        ObjectAttributeOffset = reader.Read<uint>();

        if (UseBounds)
        {
            RightBound = reader.Read<short>();
            BottomBound = reader.Read<short>();
            LeftBound = reader.Read<short>();
            TopBound = reader.Read<short>();
        }
    }
}

public class CellObject : BaseDeserializable
{
    public ushort Y;
    public bool IsTransformable;
    public bool UseDoubleSized;
    public CellObjectMode ObjectMode;
    public bool UseMosaic;
    public CellObjectPaletteType PaletteType;
    public ushort KeyShape;
    
    public ushort X;
    public bool FlipHorizontal;
    public bool FlipVertical;
    public ushort KeySize;

    public ushort TileIndex;
    public ushort Priority;
    public ushort PaletteIndex;

    public ushort TileWidth => SizeTable[KeyShape][KeySize];
    public ushort TileHeight => SizeTable[3 - KeyShape][KeySize];

    private static readonly ushort[][] SizeTable =
    [
        [1, 2, 4, 8],
        [2, 4, 4, 8],
        [1, 1, 2, 4],
        [1, 2, 4, 8]
    ];
    
    public override void Deserialize(DataReader reader)
    {
        var flag1 = reader.Read<ushort>();
        Y = flag1.Bits(0, 8);
        IsTransformable = flag1.Bit(8) == 1;
        UseDoubleSized = flag1.Bit(9) == 1;
        ObjectMode = (CellObjectMode) flag1.Bits(10, 12);
        UseMosaic = flag1.Bit(12) == 1;
        PaletteType = (CellObjectPaletteType) flag1.Bit(13);
        KeyShape = flag1.Bits(14, 16);

        var flag2 = reader.Read<ushort>();
        X = flag2.Bits(0, 9);
        FlipHorizontal = flag2.Bit(12) == 1;
        FlipVertical = flag2.Bit(13) == 1;
        KeySize = flag2.Bits(14, 16);

        var flags3 = reader.Read<ushort>();
        TileIndex = flags3.Bits(0, 10);
        Priority = flags3.Bits(10, 12);
        PaletteIndex = flags3.Bits(12, 16);
    }
}

public enum CellObjectMode : byte
{
    Normal = 0,
    SemiTransparent = 1,
    ObjectWindow = 2,
    Invalid = 3
}

public enum CellObjectPaletteType
{
    Color16,
    Color256
}