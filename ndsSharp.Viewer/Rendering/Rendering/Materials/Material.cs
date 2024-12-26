using System;
using Avalonia.Media;
using OpenTK.Graphics.OpenGL;

namespace ndsSharp.Viewer.Rendering.Rendering.Materials;

public class Material : IDisposable
{
    private Texture2D Diffuse;
    public float Alpha;
    
    public Material(Core.Conversion.Models.Mesh.Material material)
    {
        Diffuse = material.Texture is not null
            ? new Texture2D(material.Texture)
            : new Texture2D(new Color(255, 204, 204, 204));

        Alpha = material.Alpha;
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