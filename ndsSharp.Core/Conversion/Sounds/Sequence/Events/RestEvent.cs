namespace ndsSharp.Core.Conversion.Sounds.Sequence.Events;

public class RestEvent : BaseSequenceEvent
{
    [SequenceParameter(VariableLength = true)] public int Duration;
}