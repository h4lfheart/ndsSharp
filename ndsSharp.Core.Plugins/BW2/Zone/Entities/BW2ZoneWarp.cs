using ndsSharp.Core.Data;
using ndsSharp.Core.Objects;

namespace ndsSharp.Core.Plugins.BW2.Zone.Entities;

public class BW2ZoneWarp : BaseDeserializable
{
    public ushort TargetZoneIndex;
    public ushort TargetWarpIndex;
    public BW2WarpEnterDirection EnterDirection;
    public byte TransitionType;
    
    public bool UsesRailPosition;
    public ushort RailLineIndex;
    public ushort RailFrontPosition;
    public short RailSidePosition;

    public short X;
    public short Y;
    public short Z;

    public short Width;
    public short Height;
    
    public override void Deserialize(DataReader reader)
    {
        TargetZoneIndex = reader.Read<ushort>();
        TargetWarpIndex = reader.Read<ushort>();
        EnterDirection = reader.ReadEnum<BW2WarpEnterDirection>();
        TransitionType = reader.Read<byte>();
        
        UsesRailPosition = reader.Read<ushort>() == 1;
        if (UsesRailPosition)
        {
            RailLineIndex = reader.Read<ushort>();
            RailFrontPosition = reader.Read<ushort>();
            RailSidePosition = reader.Read<short>();
        }
        else
        {
            X = reader.Read<short>();
            Y = reader.Read<short>();
            Z = reader.Read<short>();
        }

        Width = reader.Read<short>();
        Height = reader.Read<short>();
        reader.Position += sizeof(ushort);
    }
}

public enum BW2WarpEnterDirection : byte
{
    All,
    North,
    South,
    West,
    East
}