namespace ndsSharp.Core.Conversion.Textures.Colors.Types;

public class BGR555 : ColorDeserializer<ushort>
{
    public static BGR555 Instance = new();
    
    public override Color ProvideColor(ushort value)
    {
        var r = (byte) (((value >> 0) & 0x1F) << 3);
        var g = (byte) (((value >> 5) & 0x1F) << 3);
        var b = (byte) (((value >> 10) & 0x1F) << 3);
        return new Color(r, g, b);
    }
}