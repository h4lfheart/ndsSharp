using ndsSharp.Core.Data;

namespace ndsSharp.Core.Objects.Exports.Sounds.Blocks;

public class SYMB : RecordBlock<string>
{
    public override string Magic => "SYMB";
    
    protected override string ReadRecord(DataReader reader, SoundFileType type)
    {
        return reader.ReadNullTerminatedString();
    }
}