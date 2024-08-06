using System.Numerics;
using SixLabors.ImageSharp.PixelFormats;

namespace ndsSharp.Conversion.Textures.Colors;

public class Color
{
    public byte R;
    public byte G;
    public byte B;
    public byte A;

    public Color(byte r, byte g, byte b, byte a = 0xFF)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }
    
    public Vector4 ToVector4() => new(R / 255f, G / 255f, B / 255f, A / 255f);

    public T ToPixel<T>() where T : IPixel, new()
    {
        var pixelColor = new T();
        pixelColor.FromVector4(ToVector4());
        return pixelColor;
    }
    

    public override string ToString()
    {
        return $"R: {R} | G: {G} | B: {B} | A: {A}";
    }
}