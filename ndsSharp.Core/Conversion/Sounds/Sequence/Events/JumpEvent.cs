namespace ndsSharp.Core.Conversion.Sounds.Sequence.Events;

public class JumpEvent : BaseSequenceEvent
{
    [SequenceParameter(ThreeByteInteger = true)] public int Offset;
}