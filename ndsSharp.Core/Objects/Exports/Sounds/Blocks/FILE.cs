using ndsSharp.Core.Data;

namespace ndsSharp.Core.Objects.Exports.Sounds.Blocks;

public class FILE : NdsBlock
{
    public uint SoundCount;
    
    public override string Magic => "FILE";

    public override void Deserialize(DataReader reader)
    {
        base.Deserialize(reader);

        SoundCount = reader.Read<uint>();
        reader.Position += sizeof(uint); // reserved
    }
}