namespace ndsSharp.Core.Conversion.Textures.Pixels.Indexed.Types;

public class A3I5 : BaseIndexed
{
    public override int BitsPerPixel => 8;

    protected override IPixel ProvidePixel(byte value)
    {
        var pixel = new IndexedPixel();
        pixel.Index = (ushort) (value & 0x1F);

        var alpha = (value >> 5) << 3;
        pixel.Alpha = (byte) ((alpha * 4 + alpha / 2) * 8);
        return pixel;
    }
}