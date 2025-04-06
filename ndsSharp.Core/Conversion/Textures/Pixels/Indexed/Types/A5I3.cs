namespace ndsSharp.Core.Conversion.Textures.Pixels.Indexed.Types;

public class A5I3 : BaseIndexed
{
    public override int BitsPerPixel => 8;

    protected override IPixel ProvidePixel(byte value)
    {
        var pixel = new IndexedPixel();
        pixel.Index = (ushort) (value & 0b111);
        pixel.Alpha = (byte) ((value >> 3) * (255f / 0b11111));
        return pixel;
    }
}