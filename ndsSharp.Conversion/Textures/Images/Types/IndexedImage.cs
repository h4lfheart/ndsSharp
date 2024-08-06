using ndsSharp.Conversion.Textures.Palettes;
using ndsSharp.Conversion.Textures.Pixels;

namespace ndsSharp.Conversion.Textures.Images.Types;

public class IndexedImage(string name, IPixel[] pixels, ImageMetaData metaData) : BaseImage(name, pixels, metaData);