using ndsSharp.Core.Data;
using ndsSharp.Core.Objects;

namespace ndsSharp.Core.Plugins.BW2.Area;

public class BW2Area : BaseDeserializable
{
    public ushort BuildingContainerIndex;
    public ushort TexturesIndex;
    public byte TextureAnimIndex;
    public byte PaletteAnimIndex;
    public bool IsExterior;
    public byte LightIndex;
    public byte OutlineType;
    
    public const int SIZE = 10;
    
    public override void Deserialize(DataReader reader)
    {
        BuildingContainerIndex = reader.Read<ushort>();
        TexturesIndex = reader.Read<ushort>();
        TextureAnimIndex = reader.Read<byte>();
        PaletteAnimIndex = reader.Read<byte>();
        IsExterior = reader.Read<byte>() == 1;
        LightIndex = reader.Read<byte>();
        OutlineType = reader.Read<byte>();
        reader.Position += 1;
    }
}