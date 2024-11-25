namespace ndsSharp.Core.Conversion.Sounds.Sequence.Events;

public class CallEvent : BaseSequenceEvent
{
    [SequenceParameter(ThreeByteInteger = true)] public int Offset;
}