using ndsSharp.Core.Data;
using ndsSharp.Core.Objects;

namespace ndsSharp.Core.Plugins.BW2.Zone.Entities;

public class BW2ZoneTrigger : BaseDeserializable
{
    public ushort ScriptIndex;
    public ushort ReferenceValue;
    public ushort Id;
    public ushort Type;
    
    public bool UsesRailPosition;
    public ushort RailLineIndex;
    public ushort RailFrontPosition;
    public short RailSidePosition;
    
    public ushort Width;
    public ushort Height;

    public short X;
    public short Z;
    public short Y;
    
    public override void Deserialize(DataReader reader)
    {
         ScriptIndex = reader.Read<ushort>();
         ReferenceValue = reader.Read<ushort>();
         Id = reader.Read<ushort>();
         Type = reader.Read<ushort>();
        
        UsesRailPosition = reader.Read<ushort>() != 0;
        if (UsesRailPosition)
        {
            RailLineIndex = reader.Read<ushort>();
            RailFrontPosition = reader.Read<ushort>();
            RailSidePosition = reader.Read<short>();

            Width = reader.Read<ushort>();
            Height = reader.Read<ushort>();
        }
        else
        {
            X = reader.Read<short>();
            Z = reader.Read<short>();
            
            Width = reader.Read<ushort>();
            Height = reader.Read<ushort>();

            Y = reader.Read<short>();
        }

        reader.Position += sizeof(ushort);
    }
}