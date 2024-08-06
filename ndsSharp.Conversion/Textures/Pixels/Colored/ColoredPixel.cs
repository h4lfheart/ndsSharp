using ndsSharp.Conversion.Textures.Colors;

namespace ndsSharp.Conversion.Textures.Pixels.Colored;

public class ColoredPixel(Color color) : IPixel
{
    public Color Color = color;
}