using System;
using System.Diagnostics;
using Avalonia.Media;
using ndsSharp.Core.Conversion.Textures.Images;
using OpenTK.Graphics.OpenGL;

namespace ndsSharp.FileExplorer.Rendering.Rendering.Materials;

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