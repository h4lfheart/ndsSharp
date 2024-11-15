using ndsSharp.Core.Data;

namespace ndsSharp.Core.Objects.Exports.Meshes.MDL;

public class MDL0Polygon : DeserializableWithName
{
    public ushort Tag;
    public ushort Length;
    public PolygonFlag Flag;
    public uint CommandOffset;
    public uint CommandLength;
    
    public List<MDL0PolygonCommand> Commands = [];
    
    public override void Deserialize(BaseReader reader)
    {
        Tag = reader.Read<ushort>();
        Length = reader.Read<ushort>();
        Flag = reader.ReadEnum<PolygonFlag, uint>();
        CommandOffset = reader.Read<uint>();
        CommandLength = reader.Read<uint>();
    }
}

public class MDL0PolygonCommand : BaseDeserializable
{
    public PolygonCommandOpCode OpCode;
    public int[] Parameters = [];

    public override void Deserialize(BaseReader reader)
    {
        var parameterCount = GetParameterCount();
        Parameters = reader.ReadArray<int>(parameterCount);
    }

    private int GetParameterCount()
    {
        return OpCode switch
        {
            PolygonCommandOpCode.NOP => 0,
            PolygonCommandOpCode.MTX_MODE => 1,
            PolygonCommandOpCode.MTX_PUSH => 0,
            PolygonCommandOpCode.MTX_POP => 1,
            PolygonCommandOpCode.MTX_STORE => 1,
            PolygonCommandOpCode.MTX_RESTORE => 1,
            PolygonCommandOpCode.MTX_IDENTITY => 0,
            PolygonCommandOpCode.MTX_LOAD_4x4 => 16,
            PolygonCommandOpCode.MTX_LOAD_4x3 => 12,
            PolygonCommandOpCode.MTX_MULT_4x4 => 16,
            PolygonCommandOpCode.MTX_MULT_4x3 => 12,
            PolygonCommandOpCode.MTX_MULT_3x3 => 9,
            PolygonCommandOpCode.MTX_SCALE => 3,
            PolygonCommandOpCode.MTX_TRANS => 3,
            PolygonCommandOpCode.COLOR => 1,
            PolygonCommandOpCode.NORMAL => 1,
            PolygonCommandOpCode.TEXCOORD => 1,
            PolygonCommandOpCode.VTX_16 => 2,
            PolygonCommandOpCode.VTX_10 => 1,
            PolygonCommandOpCode.VTX_XY => 1,
            PolygonCommandOpCode.VTX_XZ => 1,
            PolygonCommandOpCode.VTX_YZ => 1,
            PolygonCommandOpCode.VTX_DIFF => 1,
            PolygonCommandOpCode.POLYGON_ATTR => 1,
            PolygonCommandOpCode.TEXIMAGE_PARAM => 1,
            PolygonCommandOpCode.PLTT_BASE => 1,
            PolygonCommandOpCode.DIF_AMB => 1,
            PolygonCommandOpCode.SPE_EMI => 1,
            PolygonCommandOpCode.LIGHT_VECTOR => 1,
            PolygonCommandOpCode.LIGHT_COLOR => 1,
            PolygonCommandOpCode.SHININESS => 32,
            PolygonCommandOpCode.BEGIN_VTXS => 1,
            PolygonCommandOpCode.END_VTXS => 0,
            PolygonCommandOpCode.SWAP_BUFFERS => 1,
            PolygonCommandOpCode.VIEWPORT => 1,
            PolygonCommandOpCode.BOX_TEST => 3,
            PolygonCommandOpCode.POS_TEST => 2,
            PolygonCommandOpCode.VEC_TEST => 1,
            _ => 0,
        };
    }
}

[Flags]
public enum PolygonFlag : uint
{
    NORMAL = 1,
    COLOR = 2,
    TEXCOORD = 4,
    RESTORE_MATRIX = 8,
}

public enum PolygonCommandOpCode : byte
{
    NOP = 0x00,
    MTX_MODE = 0x10,
    MTX_PUSH = 0x11,
    MTX_POP = 0x12,
    MTX_STORE = 0x13,
    MTX_RESTORE = 0x14,
    MTX_IDENTITY = 0x15,
    MTX_LOAD_4x4 = 0x16,
    MTX_LOAD_4x3 = 0x17,
    MTX_MULT_4x4 = 0x18,
    MTX_MULT_4x3 = 0x19,
    MTX_MULT_3x3 = 0x1A,
    MTX_SCALE = 0x1B,
    MTX_TRANS = 0x1C,
    COLOR = 0x20,
    NORMAL = 0x21,
    TEXCOORD = 0x22,
    VTX_16 = 0x23,
    VTX_10 = 0x24,
    VTX_XY = 0x25,
    VTX_XZ = 0x26,
    VTX_YZ = 0x27,
    VTX_DIFF = 0x28,
    POLYGON_ATTR = 0x29,
    TEXIMAGE_PARAM = 0x2A,
    PLTT_BASE = 0x2B,
    DIF_AMB = 0x30,
    SPE_EMI = 0x31,
    LIGHT_VECTOR = 0x32,
    LIGHT_COLOR = 0x33,
    SHININESS = 0x34,
    BEGIN_VTXS = 0x40,
    END_VTXS = 0x41,
    SWAP_BUFFERS = 0x50,
    VIEWPORT = 0x60,
    BOX_TEST = 0x70,
    POS_TEST = 0x71,
    VEC_TEST = 0x72,
}