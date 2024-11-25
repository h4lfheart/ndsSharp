namespace ndsSharp.Core.Conversion.Sounds.Sequence.Events;

public class NoteEvent : BaseSequenceEvent
{
    [SequenceParameter] public byte Velocity;
    [SequenceParameter(VariableLength = true)] public int Duration;

    public int Note => ReadCommand; // command == key for 0..0x7F
}