using ndsSharp.Core.Data;

namespace ndsSharp.Conversion.Textures.Pixels;

public abstract class PixelDeserializer
{
    public abstract int BitsPerPixel { get; }

    public abstract IPixel[] Deserialize(BaseReader reader, int pixelCount);
}


public abstract class PixelDeserializer<T> : PixelDeserializer where T : unmanaged
{
    public override int BitsPerPixel => 0;

    public override IPixel[] Deserialize(BaseReader reader, int pixelCount)
    {
        var data = reader.GetBuffer();

        var pixels = new IPixel[pixelCount];
        
        var bitMask = (1 << BitsPerPixel) - 1;
        for (int pixelIndex = 0, bitIndex = 0; pixelIndex < pixelCount; pixelIndex++, bitIndex += BitsPerPixel) 
        {
            var value = (data[bitIndex / 8] >> bitIndex % 8) & bitMask;
            pixels[pixelIndex] = ProvidePixel((T) Convert.ChangeType(value, typeof(T)));
        }

        return pixels;
    }

    protected abstract IPixel ProvidePixel(T value);
}