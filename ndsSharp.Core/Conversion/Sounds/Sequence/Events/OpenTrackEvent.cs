using ndsSharp.Core.Data;

namespace ndsSharp.Core.Conversion.Sounds.Sequence.Events;

public class OpenTrackEvent : BaseSequenceEvent
{
    [SequenceParameter] public byte TrackNumber;
    public int Offset;

    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);

        Offset = reader.Read<byte>() | (reader.Read<byte>() << 8) | (reader.Read<byte>() << 16);
    }
}