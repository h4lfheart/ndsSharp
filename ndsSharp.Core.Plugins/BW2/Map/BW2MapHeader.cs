using ndsSharp.Core.Data;
using ndsSharp.Core.Extensions;
using ndsSharp.Core.Objects;

namespace ndsSharp.Plugins.BW2.Map;

public class BW2MapHeader : BaseDeserializable
{
    public byte MapType;
    
    public ushort AreaIndex;
    public ushort MatrixIndex;
    public ushort ScriptIndex;
    public ushort InitScriptIndex;
    public ushort TextIndex;

    public ushort SpringMusicIndex;
    public ushort SummerMusicIndex;
    public ushort AutumnMusicIndex;
    public ushort WinterMusicIndex;

    public ushort ZoneIndex;
    public ushort ParentZoneIndex;
    
    public ushort NameIndex;
    
    public const int HEADER_SIZE = 48;
    
    public override void Deserialize(DataReader reader)
    {
        MapType = reader.Read<byte>();

        reader.Position += 1;
        
        AreaIndex = reader.Read<ushort>();
        MatrixIndex = reader.Read<ushort>();
        ScriptIndex = reader.Read<ushort>();
        InitScriptIndex = reader.Read<ushort>();
        TextIndex = reader.Read<ushort>();
        
        SpringMusicIndex = reader.Read<ushort>();
        SummerMusicIndex = reader.Read<ushort>();
        AutumnMusicIndex = reader.Read<ushort>();
        WinterMusicIndex = reader.Read<ushort>();

        reader.Position += 2;

        ZoneIndex = reader.Read<ushort>();
        ParentZoneIndex = reader.Read<ushort>();

        NameIndex = reader.Read<ushort>().Bits(0, 14);

        reader.Position += sizeof(ushort) * 4;
        reader.Position += sizeof(uint) * 3;
    }
}