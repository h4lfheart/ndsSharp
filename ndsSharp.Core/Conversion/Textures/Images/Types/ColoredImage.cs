using ndsSharp.Core.Conversion.Textures.Pixels;

namespace ndsSharp.Core.Conversion.Textures.Images.Types;

public class ColoredImage(string name, IPixel[] pixels, ImageMetaData metaData) : BaseImage(name, pixels, metaData);