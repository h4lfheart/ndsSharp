using ndsSharp.Core.Conversion.Textures.Colors;

namespace ndsSharp.Core.Conversion.Textures.Pixels.Colored;

public class ColoredPixel(Color color) : IPixel
{
    public Color Color = color;
}