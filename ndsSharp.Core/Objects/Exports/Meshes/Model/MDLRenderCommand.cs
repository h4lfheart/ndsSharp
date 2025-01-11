using ndsSharp.Core.Data;

namespace ndsSharp.Core.Objects.Exports.Meshes.Model;

public class MDLRenderCommand : BaseDeserializable
{
    public RenderCommandOpCode OpCode;
    public int Flags;
    public byte[] Parameters;
    
    public override void Deserialize(DataReader reader)
    {
        var data = reader.Read<byte>();
        OpCode = (RenderCommandOpCode) (data & 0b11111);
        Flags = data >> 5;

        var parameterCount = (byte) OpCode switch
        {
            0x00 => 0,
            0x01 => 0,
            0x02 => 2,
            0x03 => 1,
            0x04 => 1,
            0x05 => 1,
            0x06 => Flags switch
            {
                0 => 3,
                1 => 4,
                2 => 4,
                3 => 5,
            },
            0x07 => 1,
            0x08 => 1,
            0x09 => 2 + 3 * reader.Peek(() =>
            {
                reader.Position += 1;
                return reader.Read<byte>();
            }),
            0x0b => 0,
            0x0c => 2,
            0x0d => 2,
            0x24 => 1,
            0x26 => 4,
            0x2b => 0,
            0x40 => 0,
            0x44 => 1,
            0x46 => 4,
            0x47 => 2,
            0x66 => 5,
            0x80 => 0
        };

        Parameters = reader.ReadArray<byte>(parameterCount);
    }
    
}

public enum RenderCommandOpCode : byte
{
    NOP = 0x00,
    END = 0x01,
    VISIBILITY = 0x02,
    LOAD_MATRIX = 0x03,
    BIND_MATERIAL = 0x04,
    DRAW_MESH = 0x05,
    MULTIPLY_MATRIX = 0x06,
    SKINNNG_EQUATION = 0x09,
    SCALE = 0x0B
}