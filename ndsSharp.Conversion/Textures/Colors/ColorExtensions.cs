using ndsSharp.Core.Data;

namespace ndsSharp.Conversion.Textures.Colors;

public static class ColorExtensions
{
    public static Color[] ReadColors<T>(this BaseReader reader, int colorCount) where T : ColorDeserializer, new()
    {
        var deserializer = new T();
        var colors = new Color[colorCount];
        for (var colorIndex = 0; colorIndex < colorCount; colorIndex++)
        {
            colors[colorIndex] = deserializer.Deserialize(reader);
        }

        return colors;
    }
    
    public static Color[] ReadColors<T>(this BaseReader reader) where T : ColorDeserializer, new()
    {
        var deserializer = new T();
        var colorCount = reader.Length / deserializer.Size;
        return ReadColors<T>(reader, colorCount);
    }
}