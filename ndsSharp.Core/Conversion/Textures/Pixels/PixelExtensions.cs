using ndsSharp.Core.Data;

namespace ndsSharp.Core.Conversion.Textures.Pixels;

public static class PixelExtensions
{
    public static IPixel[] ReadPixels<T>(this DataReader reader, int width, int height) where T : PixelDeserializer, new()
    {
        return reader.ReadPixels<T>(width * height);
    }
    
    public static IPixel[] ReadPixels<T>(this DataReader reader, int count) where T : PixelDeserializer, new()
    {
        var deserializer = new T();
        return deserializer.Deserialize(reader, count);
    }
}