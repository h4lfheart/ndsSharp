namespace ndsSharp.Core.Conversion.Sounds.Sequence.Events;

public class TransposeEvent : BaseSequenceEvent
{
    [SequenceParameter] public byte Transposition;
}