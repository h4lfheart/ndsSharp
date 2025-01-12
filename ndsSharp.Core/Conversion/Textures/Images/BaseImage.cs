using ndsSharp.Core.Conversion.Textures.Pixels;
using ndsSharp.Core.Objects.Exports.Textures;
using ndsSharp.Core.Objects.Exports.Textures.Blocks;

namespace ndsSharp.Core.Conversion.Textures.Images
{
    public class BaseImage(string name, IPixel[] pixels, ImageMetaData metaData)
    {
        public string Name = name;
        public ImageMetaData MetaData = metaData;
        public IPixel[] Pixels = pixels;
    }

    public record ImageMetaData(
        int Width,
        int Height,
        TextureFormat Format = TextureFormat.None,
        bool RepeatU = false,
        bool RepeatV = false,
        bool FlipU = false,
        bool FlipV = false,
        bool IsFirstColorTransparent = false);
}

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit {}
}