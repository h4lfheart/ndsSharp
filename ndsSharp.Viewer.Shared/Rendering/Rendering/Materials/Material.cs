using System.Diagnostics;
using Avalonia.Media;
using ndsSharp.Core.Conversion.Textures.Pixels.Colored;
using ndsSharp.Core.Conversion.Textures.Pixels.Indexed;
using OpenTK.Graphics.OpenGL;

namespace ndsSharp.Viewer.Shared.Rendering.Rendering.Materials;

public class Material : IDisposable
{
    public string Name;
    private Texture2D Diffuse;
    private float Alpha;

    public bool IsTransparent;
    
    public Material(Core.Conversion.Models.Mesh.Material material)
    {
        Name = material.Name;
        Diffuse = material.Texture is not null
            ? new Texture2D(material.Texture, material.FlipU, material.FlipV, material.RepeatU, material.RepeatV)
            : new Texture2D(new Color(255, 204, 204, 204));

        Alpha = material.Alpha;

        IsTransparent = Alpha != 1 || (material.Texture?.Pixels.Any(pixel => pixel switch
        {
            ColoredPixel coloredPixel => coloredPixel.Color.A != byte.MaxValue,
            IndexedPixel indexedPixel => indexedPixel.Alpha  != byte.MaxValue
        }) ?? false);
    }


    public void Render(Shader shader)
    {
        shader.SetUniform("diffuse", 0);
        shader.SetUniform("alpha", Alpha);
        
        Diffuse.Bind(TextureUnit.Texture0);
    }
    
    public void Dispose()
    {
        Diffuse.Dispose();
    }
}