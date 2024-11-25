namespace ndsSharp.Core.Conversion.Sounds.Sequence.Events;

public class PlayerVolumeEvent : BaseSequenceEvent
{
    [SequenceParameter] public byte Volume;
}