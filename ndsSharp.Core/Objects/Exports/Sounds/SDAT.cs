using System.ComponentModel;
using ndsSharp.Core.Data;
using ndsSharp.Core.Extensions;
using ndsSharp.Core.Objects.Exports.Sounds.Blocks;

namespace ndsSharp.Core.Objects.Exports.Sounds;

public class SDAT : NdsObject
{
    [Block] public SYMB Symbols;
    [Block] public INFO Info;
    [Block] public FAT FileAllocationTable;
    [Block] public FILE FileData;

    public BaseReader Reader;
    
    public override string Magic => "SDAT";

    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);

        Reader = reader;
    }
}

public class BaseSoundInfo : BaseDeserializable
{
    public ushort FileID;
    
    public override void Deserialize(BaseReader reader)
    {
        FileID = reader.Read<ushort>();
        
        reader.Position += sizeof(ushort);
    }
}

public enum SoundFileType
{
    [Description("SSEQ")]
    Sequence,
    
    [Description("SSAR")]
    SequenceArchive,
    
    [Description("SBNK")]
    Bank,
    
    [Description("SWAR")]
    WaveArchive,
    
    [Description("PLAYER")]
    GroupPlayer,
    
    [Description("GROUP")]
    Group,
    
    [Description("PLAYER2")]
    StreamPlayer,
    
    [Description("STRM")]
    Stream
}

public enum WaveType : byte
{
    PCM8 = 0,
    PCM16 = 1,
    ADPCM = 2
}