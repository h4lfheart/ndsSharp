using ndsSharp.Core.Data;
using ndsSharp.Core.Objects;

namespace ndsSharp.Core.Plugins.BW2.Misc;

public class BW2NPC : BaseDeserializable
{
    public ushort NPCIndex;
    public byte EntityType;
    public byte SceneNodeType;
    public bool HasShadow;
    public byte FootprintType;
    public bool HasReflections;
    public byte BillboardSize;
    public byte SpriteAtlasSize;
    public byte SpriteControllerType;
    public BW2NPCGender Gender;
    
    public byte Width;
    public byte Height;
    public byte X;
    public byte Y;
    public byte Z;
    
    public short[] ResourceIndices = [];
    
    public override void Deserialize(DataReader reader)
    {
        NPCIndex = reader.Read<ushort>();
        EntityType = reader.Read<byte>();
        SceneNodeType = reader.Read<byte>();
        HasShadow = reader.Read<byte>() == 1;
        FootprintType = reader.Read<byte>();
        HasReflections = reader.Read<byte>() == 1;
        BillboardSize = reader.Read<byte>();
        SpriteAtlasSize = reader.Read<byte>();
        SpriteControllerType = reader.Read<byte>();
        Gender = reader.ReadEnum<BW2NPCGender>();
        
        Width = reader.Read<byte>();
        Height = reader.Read<byte>();
        X = reader.Read<byte>();
        Y = reader.Read<byte>();
        Z = reader.Read<byte>();

        ResourceIndices = reader.ReadArray<short>(5);
        
        reader.Position += sizeof(ushort);
    }
}

public enum BW2NPCGender : byte
{
    Male,
    Female,
    None
}