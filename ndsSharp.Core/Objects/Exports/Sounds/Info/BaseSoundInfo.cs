using ndsSharp.Core.Data;

namespace ndsSharp.Core.Objects.Exports.Sounds.Info;

public class BaseSoundInfo : BaseDeserializable
{
    public ushort FileID;
    
    public override void Deserialize(BaseReader reader)
    {
        FileID = reader.Read<ushort>();
    }
}