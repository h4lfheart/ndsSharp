using ndsSharp.Core.Data;
using ndsSharp.Core.Extensions;

namespace ndsSharp.Core.Objects.Exports.Sounds;

public class STRM : NdsObject
{
    [Block] public HEAD Header;
    [Block] public DATA Data;
    
    public override string Magic => "STRM";

    public class HEAD : NdsBlock
    {
        public WaveType Type;
        public bool Looping;
        public ushort NumChannels;
        public ushort SampleRate;
        public ushort Time;
        public uint LoopOffset; // In Samples
        public uint NumSamples; 
        public uint DataOffset;
        public uint NumBlocks;
        
        // Per-Channel
        public uint BlockLength;
        public uint SamplesPerBlock;
        public uint LastBlockLength;
        public uint SamplesPerLastBlock;
        
        public override string Magic => "HEAD";

        public override void Deserialize(BaseReader reader)
        {
            base.Deserialize(reader);
            
            Type = reader.ReadEnum<WaveType, byte>();
            Looping = reader.Read<byte>() == 1;
            NumChannels = reader.Read<ushort>();
            SampleRate = reader.Read<ushort>();
            Time = reader.Read<ushort>();
            LoopOffset = reader.Read<uint>();
            NumSamples = reader.Read<uint>();
            DataOffset = reader.Read<uint>();
            NumBlocks = reader.Read<uint>();
            BlockLength = reader.Read<uint>();
            SamplesPerBlock = reader.Read<uint>();
            LastBlockLength = reader.Read<uint>();
            SamplesPerLastBlock = reader.Read<uint>();

            reader.Position += 32; // reserved
        }
    }
    
    public class DATA : NdsBlock
    {
        public DataPointer Data;
    
        public override string Magic => "DATA";

        public override void Deserialize(BaseReader reader)
        {
            base.Deserialize(reader);

            Data = new DataPointer((int) DataOffset, (int) DataSize, reader);
        }
    }
}