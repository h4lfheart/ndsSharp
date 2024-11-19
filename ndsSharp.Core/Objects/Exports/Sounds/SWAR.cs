using System.Diagnostics;
using ndsSharp.Core.Data;
using ndsSharp.Core.Extensions;

namespace ndsSharp.Core.Objects.Exports.Sounds;

public class SWAR : RecordObject
{
    [Block] public DATA Data;
    
    public override string Magic => "SWAR";
    
    public class DATA : NdsBlock
    {
        public uint NumWaves;

        public List<SWAVInfo> WaveInfos = [];
        public List<DataPointer> WavePointers = [];
    
        public override string Magic => "DATA";

        public override void Deserialize(BaseReader reader)
        {
            base.Deserialize(reader);

            reader.Position += sizeof(uint) * 8; // reserved

            NumWaves = reader.Read<uint>();

            var dataOffsets = reader.ReadArray<uint>((int) NumWaves);
            for (var waveIndex = 0; waveIndex < NumWaves; waveIndex++)
            {
                var info = reader.ReadObject<SWAVInfo>();
                WaveInfos.Add(info);

                var dataSize = waveIndex == NumWaves - 1
                    ? reader.Length - dataOffsets[waveIndex]
                    : dataOffsets[waveIndex + 1] - dataOffsets[waveIndex] - 12;
                
                
                WavePointers.Add(new DataPointer((int) dataOffsets[waveIndex], (int) dataSize, reader));

                reader.Position += (int) dataSize;
            }
        }
    }
}

public class SWAVInfo : BaseDeserializable
{
    public WaveType WaveType;
    public bool Loop;
    public ushort SampleRate;
    public ushort Time;
    public ushort LoopOffset;
    public uint NonLoopLength;
    
    public override void Deserialize(BaseReader reader)
    {
        WaveType = reader.ReadEnum<WaveType, byte>();
        Loop = reader.Read<byte>() == 1;
        SampleRate = reader.Read<ushort>();
        Time = reader.Read<ushort>();
        LoopOffset = reader.Read<ushort>();
        NonLoopLength = reader.Read<uint>();
    }
}