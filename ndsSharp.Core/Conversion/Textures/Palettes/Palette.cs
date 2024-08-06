using ndsSharp.Core.Conversion.Textures.Colors;

namespace ndsSharp.Core.Conversion.Textures.Palettes;

public class Palette(string name, IEnumerable<Color> colors)
{
    public string Name = name;
    public List<Color> Colors = [..colors];
    
    public bool IsBlank => Colors.All(color => color is { R: 0, G: 0, B: 0 });
}