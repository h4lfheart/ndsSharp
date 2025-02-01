using ndsSharp.Core.Data;
using ndsSharp.Core.Objects;
using ndsSharp.Core.Plugins.BW2.Zone.Entities;

namespace ndsSharp.Core.Plugins.BW2.Zone;

public class BW2ZoneEntityContainer : BaseDeserializable
{
    public BW2ZoneInteractable[] Interactables = [];
    public BW2ZoneNPC[] NPCs = [];
    public BW2ZoneWarp[] Warps = [];
    public BW2ZoneTrigger[] Triggers = [];
    
    private uint FileSize;
    private byte InteractableCount;
    private byte NPCCount;
    private byte WarpCount;
    private byte TriggerCount;
    
    public override void Deserialize(DataReader reader)
    {
        FileSize = reader.Read<uint>();
        
        InteractableCount = reader.Read<byte>();
        NPCCount = reader.Read<byte>();
        WarpCount = reader.Read<byte>();
        TriggerCount = reader.Read<byte>();

        Interactables = reader.ReadArray(InteractableCount, () => reader.ReadObject<BW2ZoneInteractable>());
        NPCs = reader.ReadArray(NPCCount, () => reader.ReadObject<BW2ZoneNPC>());
        Warps = reader.ReadArray(WarpCount, () => reader.ReadObject<BW2ZoneWarp>());
        Triggers = reader.ReadArray(TriggerCount, () => reader.ReadObject<BW2ZoneTrigger>());
    }
}