using System.Diagnostics;
using ndsSharp.Core.Data;
using ndsSharp.Core.Extensions;

namespace ndsSharp.Core.Objects.Exports.Sounds;

public class SSEQ : RecordObject<SequenceSoundInfo>
{
    [Block] public DATA Data;
    
    public override string Magic => "SSEQ";
    
    public class DATA : NdsBlock
    {
        public DataPointer SequenceByteCodePointer;
        public override string Magic => "DATA";

        public override void Deserialize(BaseReader reader)
        {
            base.Deserialize(reader);

            reader.Position += sizeof(uint); // reader position relative to sseq reader

            SequenceByteCodePointer = new DataPointer(reader.Position, reader.Length - reader.Position, reader);
        }
    }
}

public class SequenceSoundInfo : BaseSoundInfo
{
    public ushort BankIndex;
    public byte Volume;
    
    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);

        BankIndex = reader.Read<ushort>();

        Volume = reader.Read<byte>();
    }
}