using System.Numerics;
using ndsSharp.Core.Conversion.Textures.Colors;
using ndsSharp.Core.Conversion.Textures.Images;
using ndsSharp.Core.Objects.Exports.Meshes.Model;

namespace ndsSharp.Core.Conversion.Models.Mesh;

public class Material
{
    public string Name;
    public BaseImage? Texture;
    public string TextureName;
    public bool RepeatU;
    public bool RepeatV;
    public bool FlipU;
    public bool FlipV;

    public Vector2 Position;
    public Vector2 Scale;

    public Color Diffuse;
    public Color Ambient;
    public Color Specular;
    public Color Emissive;
    public float Alpha;

    public bool[] LightToggles;
    public uint PolygonID;
    public bool RenderBackFace;
    public bool RenderFrontFace;

    public static Material FromMDL(MDLMaterial materialData)
    {
        return new Material
        {
            Name = materialData.Name,
            TextureName = materialData.TextureName,
            FlipU = materialData.FlipU,
            FlipV = materialData.FlipV,
            RepeatU = materialData.RepeatU,
            RepeatV = materialData.RepeatV,
            Diffuse = materialData.Diffuse,
            Ambient = materialData.Ambient,
            Specular = materialData.Specular,
            Emissive = materialData.Emissive,
            Alpha = materialData.Alpha,
            LightToggles = materialData.LightToggles,
            PolygonID = materialData.PolygonID,
            Position = new Vector2(materialData.TransU > 0 ? materialData.TransU / 4096f : 1, materialData.TransV > 0 ? materialData.TransV / 4096f : 1),
            Scale = new Vector2(materialData.ScaleU > 0 ? materialData.ScaleU / 4096f : 1, materialData.ScaleV > 0 ? materialData.ScaleV / 4096f : 1),
            RenderBackFace = materialData.RenderBackFace,
            RenderFrontFace = materialData.RenderFrontFace
        };
    }

}