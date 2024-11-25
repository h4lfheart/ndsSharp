namespace ndsSharp.Core.Conversion.Sounds.Sequence.Events;

public class TrackVolumeEvent : BaseSequenceEvent
{
    [SequenceParameter] public byte Volume;
}