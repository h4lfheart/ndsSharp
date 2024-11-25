namespace ndsSharp.Core.Conversion.Sounds.Sequence.Events;

public class ModulationTypeEvent : BaseSequenceEvent
{
    [SequenceParameter] public ModulationType Type;
}

public enum ModulationType : byte
{
    Pitch = 0,
    Volume = 1,
    Pan = 2
}