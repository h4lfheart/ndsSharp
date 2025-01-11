using ndsSharp.Core.Conversion.Sounds.Sequence.Events;
using ndsSharp.Core.Data;
using ndsSharp.Core.Objects.Exports.Sounds;

namespace ndsSharp.Core.Conversion.Sounds.Sequence;

public static class SequenceExtensions
{
    public static List<BaseSequenceEvent>[] ReadTracks(this SSEQ sseq)
    {
        var reader = new TrackReader(sseq);
        return reader.ReadTracks();
    }
    
    public static int ReadVariableLength(this DataReader reader)
    {
        var finalValue = 0;

        var data = 0;
        var readBytes = 0;
        do
        {
            data = reader.Read<byte>();
            finalValue = (finalValue << 7) | (data & 0x7f);
            readBytes++;
        } while (readBytes < 4 && (data & 0x80) != 0);

        return finalValue;
    }
}