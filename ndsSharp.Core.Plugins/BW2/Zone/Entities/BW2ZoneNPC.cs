using ndsSharp.Core.Data;
using ndsSharp.Core.Objects;

namespace ndsSharp.Core.Plugins.BW2.Zone.Entities;

public class BW2ZoneNPC : BaseDeserializable
{
    public ushort NPCIndex;
    public ushort ObjectCode;
    public ushort MoveCode;
    public ushort EventType;
    public ushort SpawnFlag;
    public ushort ScriptIndex;
    public ushort FacingDirection;
    public ushort[] Parameters = [];
    
    public short AreaWidth;
    public short AreaHeight;
    
    public bool UsesRailPosition;
    public ushort RailLineIndex;
    public ushort RailFrontPosition;
    public short RailSidePosition;

    public ushort X;
    public ushort Z;
    public float Y;
    
    public override void Deserialize(DataReader reader)
    {
        NPCIndex = reader.Read<ushort>();
        ObjectCode = reader.Read<ushort>();
        MoveCode = reader.Read<ushort>();
        EventType = reader.Read<ushort>();
        SpawnFlag = reader.Read<ushort>();
        ScriptIndex = reader.Read<ushort>();
        FacingDirection = reader.Read<ushort>();
        Parameters = reader.ReadArray<ushort>(3);
        
        AreaWidth = reader.Read<short>();
        AreaHeight = reader.Read<short>();

        UsesRailPosition = reader.Read<int>() != 0;
        if (UsesRailPosition)
        {
            RailLineIndex = reader.Read<ushort>();
            RailFrontPosition = reader.Read<ushort>();
            RailSidePosition = reader.Read<short>();
            reader.Position += sizeof(ushort);
        }
        else
        {
            X = reader.Read<ushort>();
            Z = reader.Read<ushort>();
            Y = reader.Read<int>() / 4096f;
        }
    }
}