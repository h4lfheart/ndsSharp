using ndsSharp.Core.Data;

namespace ndsSharp.Core.Objects.Exports.Textures.Blocks;

public class CPOS : NdsBlock
{
    private ushort X;
    private ushort Y;
    private ushort Width;
    private ushort Height;
    
    public override string Magic => "CPOS";

    public override void Deserialize(DataReader reader)
    {
        base.Deserialize(reader);
        
        X = reader.Read<ushort>();
        Y = reader.Read<ushort>();
        Width = reader.Read<ushort>();
        Height = reader.Read<ushort>();
    }
}