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
            RenderBackFace = materialData.RenderBackFace,
            RenderFrontFace = materialData.RenderFrontFace
        };
    }

}