using System.Numerics;
using ndsSharp.Core.Conversion.Textures.Colors;
using ndsSharp.Core.Conversion.Textures.Colors.Types;
using ndsSharp.Core.Data;
using ndsSharp.Core.Objects;

namespace ndsSharp.Core.Plugins.BW2.Area;

public class BW2AreaLighting : BaseDeserializable
{
    public BW2AreaLightingTime LightingTime;
    public short MinuteOffset;
    
    public bool[] LightToggles = [];
    public Color[] LightColors = [];
    public Vector3[] LightDirections = [];

    public Color DiffuseColor;
    public Color AmbientColor;
    public Color SpecularColor;
    public Color EmissionColor;
    public Color FogColor;
    public Color ClearColor;
    
    public const int SIZE = 0x34;
    
    public override void Deserialize(DataReader reader)
    {
        LightingTime = reader.ReadEnum<BW2AreaLightingTime>();
        MinuteOffset = reader.Read<short>();
        LightToggles = reader.ReadArray<bool>(4);
        LightColors = reader.ReadArray(4, reader.ReadColor<BGR555>);
        LightDirections = reader.ReadArray(4, () => new Vector3(
            reader.Read<short>() / 4096f, 
            reader.Read<short>() / 4096f,
            reader.Read<short>() / 4096f)
        );
        
        DiffuseColor = reader.ReadColor<BGR555>();
        AmbientColor = reader.ReadColor<BGR555>();
        SpecularColor = reader.ReadColor<BGR555>();
        EmissionColor = reader.ReadColor<BGR555>();
        FogColor = reader.ReadColor<BGR555>();
        ClearColor = reader.ReadColor<BGR555>();

    }
}

public enum BW2AreaLightingTime : ushort
{
    Morning,
    Day,
    Sunset,
    Evening,
    Night
}