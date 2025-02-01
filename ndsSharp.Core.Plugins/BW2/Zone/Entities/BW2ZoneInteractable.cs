using ndsSharp.Core.Data;
using ndsSharp.Core.Objects;

namespace ndsSharp.Core.Plugins.BW2.Zone.Entities;

public class BW2ZoneInteractable : BaseDeserializable
{
    public ushort ScriptIndex;
    public ushort Condition;
    public ushort Interactability;
    
    public bool UsesRailPosition;
    public ushort RailLineIndex;
    public ushort RailFrontPosition;
    public short RailSidePosition;

    public int X;
    public int Z;
    public int Y;
    
    public override void Deserialize(DataReader reader)
    {
        ScriptIndex = reader.Read<ushort>();
        Condition = reader.Read<ushort>();
        Interactability = reader.Read<ushort>();

        UsesRailPosition = reader.Read<ushort>() != 0;
        if (UsesRailPosition)
        {
            RailLineIndex = reader.Read<ushort>();
            RailFrontPosition = reader.Read<ushort>();
            RailSidePosition = reader.Read<short>();
            reader.Position += sizeof(ushort);
        }
        else
        {
            X = reader.Read<int>();
            Z = reader.Read<int>();
        }
        
        Y = reader.Read<int>();
    }
}