namespace ndsSharp.Conversion.Textures.Pixels.Indexed;

public class IndexedPixel(ushort index = 0, byte alpha = 255, byte paletteIndex = 0) : IPixel
{
    public ushort Index = index;
    public byte Alpha = alpha;
    public byte PaletteIndex = paletteIndex;
}