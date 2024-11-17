using ndsSharp.Core.Conversion.Textures.Colors;
using ndsSharp.Core.Conversion.Textures.Colors.Types;
using ndsSharp.Core.Conversion.Textures.Images;
using ndsSharp.Core.Conversion.Textures.Images.Types;
using ndsSharp.Core.Conversion.Textures.Palettes;
using ndsSharp.Core.Conversion.Textures.Pixels;
using ndsSharp.Core.Conversion.Textures.Pixels.Colored.Types;
using ndsSharp.Core.Conversion.Textures.Pixels.Indexed.Types;
using ndsSharp.Core.Data;
using ndsSharp.Core.Extensions;

namespace ndsSharp.Core.Objects.Exports.Textures.Blocks;

public class TEX : NdsBlock
{
    public List<BaseImage> Textures = [];
    
    private readonly Dictionary<string, TextureInfo> _textureInfos = [];
    private readonly Dictionary<string, DataPointer> _texturePointers = [];
    private readonly Dictionary<string, DataPointer> _palettePointers = [];
    
    public override string Magic => "TEX";
    
    private uint _textureDataOffset;
    private ushort _textureDataSize;
    private ushort _textureInfoOffset;
    private uint _paletteDataOffset;
    private uint _paletteDataSize;
    private uint _paletteInfoOffset;

    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);
        
        DeserializeHeader(reader);
        DeserializeTextures(reader);
    }
    
    private void DeserializeHeader(BaseReader reader)
    {
        reader.Position += sizeof(uint); // padding
        _textureDataSize = reader.Read<ushort>();
        _textureInfoOffset = reader.Read<ushort>();
        reader.Position += sizeof(uint); // padding
        _textureDataOffset = reader.Read<uint>();
        reader.Position += sizeof(uint); // padding
        reader.Position += sizeof(ushort); // compressed data size
        reader.Position += sizeof(ushort); // compressed info offset
        reader.Position += sizeof(uint); // padding
        reader.Position += sizeof(uint); // compressed data offset
        reader.Position += sizeof(uint); // compressed info data offset
        reader.Position += sizeof(uint); // padding
        _paletteDataSize = reader.Read<uint>() << 3;
        _paletteInfoOffset = reader.Read<uint>();
        _paletteDataOffset = reader.Read<uint>();
    }
    
    private void DeserializeTextures(BaseReader reader)
    {
        var textureInfos = reader.ReadNameList<TextureInfo>();
        for (var textureIndex = 0; textureIndex < textureInfos.Count; textureIndex++)
        {
            var (textureName, textureInfo) = textureInfos[textureIndex];

            var pixelPointer = new DataPointer((int)(textureInfo.TextureOffset * 8 + _textureDataOffset),
                (int) (textureInfo.Width * textureInfo.Height * textureInfo.Format.BitsPerPixel() / 8f), reader);

            _textureInfos[textureName] = textureInfo;
            _texturePointers[textureName] = pixelPointer;
        }
        
        var paletteInfos = reader.ReadNameList(() =>
        {
            var offset = reader.Read<ushort>();
            reader.Position += sizeof(ushort); // reserved
            return offset;
        });

        paletteInfos.Items = paletteInfos.OrderBy(pair => pair.Value).ToDictionary();
        
        for (var paletteIndex = 0; paletteIndex < paletteInfos.Count; paletteIndex++)
        {
            var (paletteName, paletteOffset) = paletteInfos[paletteIndex];
            paletteOffset <<= 3;

            var nextPaletteIndex = paletteIndex;
            var nextPaletteOffset = paletteOffset;
            while (nextPaletteOffset == paletteOffset)
            {
                nextPaletteIndex++;

                if (nextPaletteIndex == paletteInfos.Count)
                {
                    nextPaletteOffset = (ushort) _paletteDataSize;
                    break;
                }
                
                nextPaletteOffset = (ushort) (paletteInfos[nextPaletteIndex].Value << 3);
            }

            var paletteSize = nextPaletteOffset - paletteOffset;
            if (paletteSize < 0) throw new ParserException($"Invalid palette size: {paletteSize}");

            _palettePointers[paletteName] = new DataPointer((int) (paletteOffset + _paletteDataOffset), paletteSize, reader);
        }
        
        for (var textureIndex = 0; textureIndex < _textureInfos.Count; textureIndex++)
        {
            var (textureName, texturePointer) = _texturePointers.ElementAt(textureIndex);
            var textureInfo = _textureInfos[textureName];
            
            var (paletteName, palettePointer) = _palettePointers.FirstOrDefault(pair => pair.Key.Equals(textureName + "_pl") || pair.Key.Equals(textureName), _palettePointers.ElementAt(Math.Min(textureIndex, _palettePointers.Count - 1)));
            var paletteReader = palettePointer.Load();
            var palette = new Palette(paletteName, paletteReader.ReadColors<BGR555>());
            
            var pixelReader = texturePointer.Load();
            var pixels = textureInfo.Format switch
            {
                TextureFormat.Color4 => pixelReader.ReadPixels<Indexed2>(textureInfo.Width, textureInfo.Height),
                TextureFormat.Color16 => pixelReader.ReadPixels<Indexed4>(textureInfo.Width, textureInfo.Height),
                TextureFormat.Color256 => pixelReader.ReadPixels<Indexed8>(textureInfo.Width, textureInfo.Height),
                TextureFormat.A3I5 => pixelReader.ReadPixels<A3I5>(textureInfo.Width, textureInfo.Height),
                TextureFormat.A5I3 => pixelReader.ReadPixels<A5I3>(textureInfo.Width, textureInfo.Height),
                TextureFormat.A1BGR5 => pixelReader.ReadPixels<A1BGR555>(textureInfo.Width, textureInfo.Height)
            };
            
            var meta = new ImageMetaData(textureInfo.Width, textureInfo.Height, textureInfo.Format, 
                textureInfo.RepeatU, textureInfo.RepeatV, textureInfo.FlipU, textureInfo.FlipV, 
                textureInfo.FirstColorIsTransparent);

            if (textureInfo.Format.IsIndexed())
            {
                Textures.Add(new IndexedPaletteImage(textureName, pixels, [palette], meta));
            }
            else
            {
                Textures.Add(new ColoredImage(textureName, pixels, meta));
            }
        }
    }
}

public class TextureInfo : BaseDeserializable
{
    public ushort TextureOffset;
    public TextureFormat Format;
    public int Height;
    public int Width;
    public bool FirstColorIsTransparent;
    public bool RepeatU;
    public bool RepeatV;
    public bool FlipU;
    public bool FlipV;

    public override void Deserialize(BaseReader reader)
    {
        TextureOffset = reader.Read<ushort>();

        var flags = reader.Read<ushort>();
        RepeatU = ((flags >> 0) & 1) == 1;
        RepeatV = ((flags >> 1) & 1) == 1;
        FlipU = ((flags >> 2) & 1) == 1;
        FlipV = ((flags >> 3) & 1) == 1;
        FirstColorIsTransparent = ((flags >> 13) & 1) != 0;
        Format = (TextureFormat) ((flags >> 10) & 7);
        Height = 8 << ((flags >> 7) & 7);
        Width = 8 << ((flags >> 4) & 7);

        reader.Position += 4;
    }
}

public enum TextureFormat : byte
{
    None = 0,
    A3I5 = 1,
    Color4 = 2,
    Color16 = 3,
    Color256 = 4,
    Texel = 5,
    A5I3 = 6,
    A1BGR5 = 7
}