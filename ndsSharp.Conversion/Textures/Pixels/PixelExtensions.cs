using ndsSharp.Core.Data;

namespace ndsSharp.Conversion.Textures.Pixels;

public static class PixelExtensions
{
    public static IPixel[] ReadPixels<T>(this BaseReader reader, int width, int height) where T : PixelDeserializer, new()
    {
        var deserializer = new T();
        return deserializer.Deserialize(reader, width * height);
    }
}