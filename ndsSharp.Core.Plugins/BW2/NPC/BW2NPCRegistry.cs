using ndsSharp.Core.Data;
using ndsSharp.Core.Objects;

namespace ndsSharp.Core.Plugins.BW2.Misc;

public class BW2NPCRegistry : BaseDeserializable
{
    public BW2NPC[] NPCs = [];
    
    public override void Deserialize(DataReader reader)
    {
        NPCs = reader.ReadArray(() => reader.ReadObject<BW2NPC>());
    }
}