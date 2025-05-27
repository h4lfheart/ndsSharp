using ndsSharp.Core.Data;
using ndsSharp.Core.Objects;

namespace ndsSharp.Core.Plugins.BW2.Map;

public class BW2MapReplacement : BaseDeserializable
{
    public ushort MatrixIndex;
    public bool IsMatrixReplacement;
    public byte Condition;
    public ushort[] Values = new ushort[5];

    public const byte CONDITION_SEASON = 0;
    public const byte CONDITION_VERSION = 1;
    public const byte CONDITION_INVALID = 0xFF;
    
    public const int SIZE = 16; 
    
    public override void Deserialize(DataReader reader)
    {
        MatrixIndex = reader.Read<ushort>();
        IsMatrixReplacement = reader.Read<bool>();
        Condition = reader.Read<byte>();
        
        for (var valueIndex = 0; valueIndex < 5; valueIndex++)
        {
            Values[valueIndex] = reader.Read<ushort>();
        }

        reader.Position += sizeof(ushort);
    }
}