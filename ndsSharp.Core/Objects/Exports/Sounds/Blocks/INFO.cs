using ndsSharp.Core.Data;
using ndsSharp.Core.Objects.Exports.Sounds.Info;

namespace ndsSharp.Core.Objects.Exports.Sounds.Blocks;

public class INFO : RecordBlock<BaseSoundInfo>
{
    public override string Magic => "INFO";
    
    protected override BaseSoundInfo ReadRecord(BaseReader reader, SoundFileType type)
    {
        return type switch
        {
            _ => reader.ReadObject<BaseSoundInfo>()
        };
    }
}