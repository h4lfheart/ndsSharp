namespace ndsSharp.Core.Conversion.Sounds.Sequence.Events;

public class InstrumentEvent : BaseSequenceEvent
{
    [SequenceParameter(VariableLength = true)] public int InstrumentIndex;
}