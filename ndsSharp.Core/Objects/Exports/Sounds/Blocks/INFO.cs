using ndsSharp.Core.Data;

namespace ndsSharp.Core.Objects.Exports.Sounds.Blocks;

public class INFO : RecordBlock<BaseSoundInfo>
{
    public override string Magic => "INFO";
    
    protected override BaseSoundInfo ReadRecord(DataReader reader, SoundFileType type)
    {
        return type switch
        {
            SoundFileType.Bank => reader.ReadObject<BankSoundInfo>(),
            SoundFileType.Stream => reader.ReadObject<StreamSoundInfo>(),
            SoundFileType.Sequence => reader.ReadObject<SequenceSoundInfo>(),
            _ => reader.ReadObject<BaseSoundInfo>()
        };
    }
}