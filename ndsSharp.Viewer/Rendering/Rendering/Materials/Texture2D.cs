using System.Runtime.CompilerServices;
using ndsSharp.Core.Conversion.Textures.Images;
using OpenTK.Graphics.OpenGL;
using SixLabors.ImageSharp.PixelFormats;
using Color = Avalonia.Media.Color;

namespace ndsSharp.Viewer.Rendering.Rendering.Materials;

public class Texture2D
{
    private readonly int Handle;
    
    public Texture2D(BaseImage image)
    {
        Handle = GL.GenTexture();
        Bind();
        
        var texture = image.ToImage();
        
        var pixelData = new byte[texture.Width * texture.Height * Unsafe.SizeOf<Rgba32>()];
        texture.CopyPixelDataTo(pixelData);

        GL.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, texture.Width, texture.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixelData);

        GL.TextureParameteri(Handle, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Nearest);
        GL.TextureParameteri(Handle, TextureParameterName.TextureMagFilter, (int) TextureMinFilter.Nearest);
        
        if (image.MetaData.FlipU)
            GL.TextureParameteri(Handle, TextureParameterName.TextureWrapS, (int) TextureWrapMode.MirroredRepeat);
        
        if (image.MetaData.FlipV)
            GL.TextureParameteri(Handle, TextureParameterName.TextureWrapT, (int) TextureWrapMode.MirroredRepeat);
        
        if (image.MetaData.RepeatU)
            GL.TextureParameteri(Handle, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Repeat);
        
        if (image.MetaData.RepeatV)
            GL.TextureParameteri(Handle, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Repeat);
    }
    
    public Texture2D(Color color)
    {
        Handle = GL.GenTexture();
        Bind();

        GL.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgb, 1, 1, 0, PixelFormat.Rgb, PixelType.Float, new[] { color.R, color.G, color.B });

        GL.TextureParameteri(Handle, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear);
        GL.TextureParameteri(Handle, TextureParameterName.TextureMagFilter, (int) TextureMinFilter.Linear);
        GL.TextureParameteri(Handle, TextureParameterName.TextureWrapS, (int) TextureWrapMode.ClampToEdge);
        GL.TextureParameteri(Handle, TextureParameterName.TextureWrapT, (int) TextureWrapMode.ClampToEdge);
    }


    public void Bind(TextureUnit unit)
    {
        GL.ActiveTexture(unit);
        Bind();
    }

    public void Bind()
    {
        GL.BindTexture(TextureTarget.Texture2d, Handle);
    }

    public void Dispose()
    {
        GL.DeleteTexture(Handle);
    }
}