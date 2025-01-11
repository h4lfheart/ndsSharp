using ndsSharp.Core.Conversion.Sounds.Sequence.Events;
using ndsSharp.Core.Data;
using ndsSharp.Core.Objects;
using ndsSharp.Core.Objects.Exports.Sounds;

namespace ndsSharp.Core.Conversion.Sounds.Sequence;

public class TrackReader(SSEQ sequence)
{
    private readonly DataReader _reader = sequence.Data.SequenceByteCodePointer.Load();

    public List<BaseSequenceEvent>[] ReadTracks()
    {
        var tracks = new List<BaseSequenceEvent>[16];
        while (true)
        {
            var command = _reader.Read<byte>();

            var readType = command switch
            {
                0x93 => typeof(OpenTrackEvent),
                0xFE => typeof(TrackAllocationEvent),
                _ => null
            };
            
            if (readType is null) break;

            var sequenceEvent = _reader.ReadObject<BaseSequenceEvent>(readType, eventData => eventData.ReadCommand = command);

            if (sequenceEvent is TrackAllocationEvent trackAllocationEvent)
            {
                var validTrackCount = 0;
                for (var bitIndex = 0; bitIndex < 16; bitIndex++)
                {
                    if (((trackAllocationEvent.UsedTrackBits >> bitIndex) & 1) == 1)
                    {
                        tracks[bitIndex] = [];
                        validTrackCount++;
                    }
                }
                
                // read track 0, no explicit open track event
                _reader.Peek(() =>
                {
                    _reader.Position = 0x3 + validTrackCount * 0x5;
                    tracks[0] = ReadTrack();
                });
            }
            else if (sequenceEvent is OpenTrackEvent openTrackEvent)
            {
                _reader.Peek(() =>
                {
                    _reader.Position = openTrackEvent.Offset;
                    tracks[openTrackEvent.TrackNumber] = ReadTrack();
                });
            }
            
        }
        

        return tracks;
    }

    private List<BaseSequenceEvent> ReadTrack()
    {
        var events = new List<BaseSequenceEvent>();
        
        BaseSequenceEvent sequenceEvent = null;
        while (sequenceEvent is not EndEvent)
        {
            var command = _reader.Read<byte>();

            var readType = command switch
            {
                <= 0x7F => typeof(NoteEvent),
                0x80 => typeof(RestEvent),
                0x81 => typeof(InstrumentEvent),
                0x94 => typeof(JumpEvent),
                0x95 => typeof(CallEvent),
                0xC7 => typeof(MonoEvent),
                0xE1 => typeof(TempoEvent),
                0xC0 => typeof(PanningEvent),
                0xC1 => typeof(TrackVolumeEvent),
                0xC2 => typeof(PlayerVolumeEvent),
                0xC3 => typeof(TransposeEvent),
                0xC4 => typeof(PitchBendEvent),
                0xC5 => typeof(PitchBendRangeEvent),
                0xC6 => typeof(TrackPriorityEvent),
                0xCA => typeof(ModulationDepthEvent),
                0xCB => typeof(ModulationSpeedEvent),
                0xCC => typeof(ModulationTypeEvent),
                0xCD => typeof(ModulationRangeEvent),
                0xCE => typeof(PortamentoToggleEvent),
                0xCF => typeof(PortamentoTimeEvent),
                0xD5 => typeof(TrackExpressionEvent),
                0xE0 => typeof(ModulationDelayEvent),
                0xFD => typeof(ReturnEvent),
                0xFF => typeof(EndEvent),
                _ => throw new Exception($"Unknown Sequence Command: {command:X2}")
            };

            sequenceEvent = _reader.ReadObject<BaseSequenceEvent>(readType, eventData => eventData.ReadCommand = command);
                
            events.Add(sequenceEvent);
        }

        return events;
    }
    
}