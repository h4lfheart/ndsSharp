namespace ndsSharp.Core.Conversion.Textures.Pixels.Indexed.Types;

public abstract class BaseIndexed : PixelDeserializer<byte>
{
    protected override IPixel ProvidePixel(byte value) => new IndexedPixel(value);
}