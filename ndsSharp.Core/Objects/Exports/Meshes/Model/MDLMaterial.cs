using ndsSharp.Core.Conversion.Textures.Colors;
using ndsSharp.Core.Conversion.Textures.Colors.Types;
using ndsSharp.Core.Data;
using ndsSharp.Core.Extensions;

namespace ndsSharp.Core.Objects.Exports.Meshes.Model;

public class MDLMaterial : DeserializableWithName
{
    public string TextureName;
    public string PaletteName;

    public Color Diffuse;
    public Color Ambient;
    public Color Specular;
    public Color Emissive;
    public float Alpha;
    public uint PolygonID;
    public bool[] LightToggles = new bool[4];
    public bool RenderBackFace;
    public bool RenderFrontFace;
    
    public ushort Tag;
    public ushort Length;
    public int DiffuseAmbient;
    public int SpecularEmissive;
    public uint PolyAttr;
    public uint PolyAttrMask;
    public ushort TextureVRAMOffset;
    public ushort TextureImageParam;
    public uint TextureImageParamMask;
    public ushort TexturePaletteBase;
    public MaterialFlag Flag;
    public ushort Width;
    public ushort Height;
    public float MagWidth;
    public float MagHeight; 
    public uint ScaleU;
    public uint ScaleV;
    public ushort RotSin;
    public ushort RotCos;
    public uint TransU;
    public uint TransV;
    public uint[] EffectMatrix = [];
    
    public bool RepeatU;
    public bool RepeatV;
    public bool FlipU;
    public bool FlipV;
    
    public override void Deserialize(DataReader reader)
    {
        Tag = reader.Read<ushort>();
        Length = reader.Read<ushort>();
        
        DiffuseAmbient = reader.Read<int>();
        Diffuse = BGR555.Instance.ProvideColor((ushort) DiffuseAmbient.Bits(0, 15));
        Ambient = BGR555.Instance.ProvideColor((ushort) DiffuseAmbient.Bits(16, 31));
        
        SpecularEmissive = reader.Read<int>();
        Specular = BGR555.Instance.ProvideColor((ushort) SpecularEmissive.Bits(0, 15));
        Emissive = BGR555.Instance.ProvideColor((ushort) SpecularEmissive.Bits(16, 31));
        
        PolyAttr = reader.Read<uint>();
        for (var bit = 0; bit < 4; bit++)
        {
            LightToggles[bit] = PolyAttr.Bit(bit) == 1;
        }
        RenderBackFace = PolyAttr.Bit(6) == 1;
        RenderFrontFace = PolyAttr.Bit(7) == 1;
        Alpha = PolyAttr.Bits(16,21) / 31.0f;
        PolygonID = PolyAttr.Bits(24, 30);
        
        PolyAttrMask = reader.Read<uint>();
        TextureVRAMOffset = reader.Read<ushort>();
        TextureImageParam = reader.Read<ushort>();
        RepeatU = ((TextureImageParam >> 0) & 1) == 1;
        RepeatV = ((TextureImageParam >> 1) & 1) == 1;
        FlipU = ((TextureImageParam >> 2) & 1) == 1;
        FlipV = ((TextureImageParam >> 3) & 1) == 1;
        TextureImageParamMask = reader.Read<uint>();
        TexturePaletteBase = reader.Read<ushort>();
        Flag = (MaterialFlag) (~reader.Read<ushort>() & 0x3FFF) ^ MaterialFlag.EFFECT_MATRIX;
        Width = reader.Read<ushort>();
        Height = reader.Read<ushort>();
        MagWidth = reader.ReadIntAsFloat();
        MagHeight = reader.ReadIntAsFloat();
        
        if (Flag.HasFlag(MaterialFlag.TEX_MATRIX_SCALEONE))
        {
            ScaleU = reader.Read<uint>();
            ScaleV = reader.Read<uint>();
        }
        if (Flag.HasFlag(MaterialFlag.TEX_MATRIX_ROTZERO))
        {
            RotSin = reader.Read<ushort>();
            RotCos = reader.Read<ushort>();
        }
        if (Flag.HasFlag(MaterialFlag.TEX_MATRIX_TRANSZERO))
        {
            TransU = reader.Read<uint>();
            TransV = reader.Read<uint>();
        }
        
        if (Flag.HasFlag(MaterialFlag.EFFECT_MATRIX))
        {
            EffectMatrix = new uint[0x10];
            for (var i = 0; i < 0x10; i++)
            {
                EffectMatrix[i] = reader.Read<uint>();
            }
        }
    }
}

[Flags]
public enum MaterialFlag : ushort
{
    TEX_MATRIX_USE = 1,
    TEX_MATRIX_SCALEONE = 2,
    TEX_MATRIX_ROTZERO = 4,
    TEX_MATRIX_TRANSZERO = 8,
                
    ORIGWH_SAME = 16, // 0x0010
    WIREFRAME = 32, // 0x0020
    DIFFUSE = 64, // 0x0040
    AMBIENT = 128, // 0x0080
                
    VTXCOLOR = 256, // 0x0100
    SPECULAR = 512, // 0x0200
    EMISSION = 1024, // 0x0400
    SHININESS = 2048, // 0x0800
                
    TEXPLTTBASE = 4096, // 0x1000
    EFFECT_MATRIX = 8192 // 0x2000 
}

public class MDLMaterialMapping : BaseDeserializable
{
    public ushort Offset;
    public byte NumMaterials;
    public byte Bound;

    public override void Deserialize(DataReader reader)
    {
        Offset = reader.Read<ushort>();
        NumMaterials = reader.Read<byte>();
        reader.Position += 1;
    }
}