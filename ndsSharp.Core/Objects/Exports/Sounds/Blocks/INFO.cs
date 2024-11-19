using ndsSharp.Core.Data;

namespace ndsSharp.Core.Objects.Exports.Sounds.Blocks;

public class INFO : RecordBlock<BaseSoundInfo>
{
    public override string Magic => "INFO";
    
    protected override BaseSoundInfo ReadRecord(BaseReader reader, SoundFileType type)
    {
        return type switch
        {
            SoundFileType.Bank => reader.ReadObject<BankSoundInfo>(),
            SoundFileType.Stream => reader.ReadObject<StreamSoundInfo>(),
            _ => reader.ReadObject<BaseSoundInfo>()
        };
    }
}