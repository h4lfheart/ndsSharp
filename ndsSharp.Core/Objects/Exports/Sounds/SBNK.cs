using System.Diagnostics;
using ndsSharp.Core.Data;
using ndsSharp.Core.Extensions;

namespace ndsSharp.Core.Objects.Exports.Sounds;

public class SBNK : RecordObject<BankSoundInfo>
{
    [Block] public DATA Data;
    
    public override string Magic => "SBNK";
    
    public class DATA : NdsBlock
    {
        public uint NumInstruments;
        public List<Instrument> Instruments = [];
        
        public override string Magic => "DATA";

        public override void Deserialize(DataReader reader)
        {
            base.Deserialize(reader);

            reader.Position += sizeof(uint) * 8; // reserved

            NumInstruments = reader.Read<uint>();

            for (var instrumentIndex = 0; instrumentIndex < NumInstruments; instrumentIndex++)
            {
                var instrument = reader.ReadObject<Instrument>();
                if (instrument.DataOffset == 0) continue;
                
                Instruments.Add(instrument);
            }
        }
    }
}

public class BankSoundInfo : BaseSoundInfo
{
    public ushort[] WaveArchiveIDs = [];
    
    public override void Deserialize(DataReader reader)
    {
        base.Deserialize(reader);

        WaveArchiveIDs = reader.ReadArray<ushort>(4);
    }
}

public class Instrument : BaseDeserializable
{
    public InstrumentType Type;
    public ushort DataOffset;
    public BaseInstrumentData Data;
    
    public override void Deserialize(DataReader reader)
    {
        Type = reader.ReadEnum<InstrumentType>();
        DataOffset = reader.Read<ushort>();

        reader.Position += 1;
        
        if (DataOffset == 0) return;

        reader.Peek(() =>
        {
            reader.Position = DataOffset - reader.Owner!.Position; // data offset is relative to SBNK start

            Data = Type switch
            {
                InstrumentType.Drum => reader.ReadObject<DrumInstrumentData>(),
                InstrumentType.KeyRegion => reader.ReadObject<KeyRegionInstrumentData>(),
                _ => reader.ReadObject<DefaultInstrumentData>()
            };
        });
    }

    public T GetData<T>() where T : BaseInstrumentData
    {
        return (T) Data;
    }
}

public class SubInstrument : BaseDeserializable
{
    public InstrumentType Type;
    public InstrumentInfo Info;
    
    public override void Deserialize(DataReader reader)
    {
        Type = reader.ReadEnum<InstrumentType>();

        reader.Position += 1;
        
        Info = reader.ReadObject<InstrumentInfo>();
    }
}

public class InstrumentInfo : BaseDeserializable
{
    public ushort WaveIndex;
    public ushort ArchiveIndex;
    public byte Key;
    public byte Attack;
    public byte Decay;
    public byte Sustain;
    public byte Release;
    public byte Pan;
    
    public override void Deserialize(DataReader reader)
    {
        WaveIndex = reader.Read<ushort>();
        ArchiveIndex = reader.Read<ushort>();
        Key = reader.Read<byte>();
        Attack = reader.Read<byte>();
        Decay = reader.Read<byte>();
        Sustain = reader.Read<byte>();
        Release = reader.Read<byte>();
        Pan = reader.Read<byte>();
    }
}


public abstract class BaseInstrumentData : BaseDeserializable
{
    
}

public class DefaultInstrumentData : BaseInstrumentData
{
    public InstrumentInfo Info;
    
    public override void Deserialize(DataReader reader)
    {
        Info = reader.ReadObject<InstrumentInfo>();
    }
}

public class DrumInstrumentData : BaseInstrumentData
{
    public byte MinNote;
    public byte MaxNote;
    public List<SubInstrument> Instruments = [];
    
    public override void Deserialize(DataReader reader)
    {
        MinNote = reader.Read<byte>();
        MaxNote = reader.Read<byte>();

        var instrumentCount = MaxNote - MinNote + 1;
        for (var instrumentIndex = 0; instrumentIndex < instrumentCount; instrumentIndex++)
        {
            var instrument = reader.ReadObject<SubInstrument>();
            Instruments.Add(instrument);
        }
    }
}

public class KeyRegionInstrumentData : BaseInstrumentData
{
    public List<SubInstrument> Instruments = [];
    public byte[] KeyRegions = [];
    
    public override void Deserialize(DataReader reader)
    {
        KeyRegions = reader.ReadArray<byte>(8);
        
        var instrumentCount = KeyRegions.TakeWhile(keyRegion => keyRegion != 0).Count();
        for (var instrumentIndex = 0; instrumentIndex < instrumentCount; instrumentIndex++)
        {
            var instrument = reader.ReadObject<SubInstrument>();
            Instruments.Add(instrument);
        }
    }
}

public enum InstrumentType : byte
{
    PCM = 0x1,
    PSG = 0x2,
    Noise = 0x3,
    Drum = 0x10,
    KeyRegion = 0x11
}