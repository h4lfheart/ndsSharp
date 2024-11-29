using ndsSharp.Core.Conversion.Textures.Images;

namespace ndsSharp.Core.Conversion.Models.Mesh;

public class Material
{
    public string Name;
    public BaseImage? Texture;
    public bool RepeatU;
    public bool RepeatV;
    public bool FlipU;
    public bool FlipV;
    public float Alpha;

}