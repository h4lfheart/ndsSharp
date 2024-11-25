namespace ndsSharp.Core.Conversion.Sounds.Sequence.Events;

public class TrackAllocationEvent : BaseSequenceEvent
{
    [SequenceParameter] public ushort UsedTrackBits;
}