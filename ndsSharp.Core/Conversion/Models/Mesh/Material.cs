using ndsSharp.Core.Conversion.Textures.Colors;
using ndsSharp.Core.Conversion.Textures.Images;

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

}