using ndsSharp.Core.Data;

namespace ndsSharp.Core.Objects.Exports.Sounds.Blocks;

public abstract class RecordBlock<TRecordType> : NdsBlock
{
    public Dictionary<SoundFileType, List<TRecordType>> Records = new();

    public override void Deserialize(DataReader reader)
    {
        base.Deserialize(reader);

        var recordOffsets = new Dictionary<SoundFileType, uint>();
        for (var recordIndex = 0; recordIndex < 8; recordIndex++)
        {
            var type = (SoundFileType) recordIndex;
            recordOffsets[type] = reader.Read<uint>();
        }

        foreach (var (type, offset) in recordOffsets)
        {
            Records[type] = ReadRecords(reader, type, offset);
        }
    }
    
    protected abstract TRecordType ReadRecord(DataReader reader, SoundFileType type);
    
    private List<TRecordType> ReadRecords(DataReader reader, SoundFileType type, uint offset)
    {
        reader.Position = (int) offset;
        
        var count = reader.Read<uint>();
        var offsets = new List<uint>();
        for (var index = 0; index < count; index++)
        {
            var entryOffset = reader.Read<uint>();
            if (entryOffset != 0)
            {
                offsets.Add(entryOffset);
            }
        }
        
        var records = new List<TRecordType>();
        foreach (var entryOffset in offsets)
        {
            reader.Position = (int) entryOffset;
            records.Add(ReadRecord(reader, type));
        }

        return records;
    }
    
}