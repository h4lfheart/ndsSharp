using ndsSharp.Core.Conversion.Textures.Colors;
using ndsSharp.Core.Conversion.Textures.Colors.Types;
using ndsSharp.Core.Conversion.Textures.Images;
using ndsSharp.Core.Conversion.Textures.Images.Types;
using ndsSharp.Core.Conversion.Textures.Palettes;
using ndsSharp.Core.Conversion.Textures.Pixels;
using ndsSharp.Core.Conversion.Textures.Pixels.Indexed.Types;
using ndsSharp.Core.Data;
using ndsSharp.Core.Data.Checksum;
using SixLabors.ImageSharp;

namespace ndsSharp.Core.Objects.Rom;

public class RomBanner
{
    public Version Version;
    public CRC16 BannerCrc16;
    public IndexedPaletteImage Icon;
    public AnimatedBannerIcon AnimatedIcon;
    public LocalizedBannerTitles Titles;
    
    private const int IconWidth = 32;
    private const int IconHeight = 32;

    private const int AnimatedIconOffset = 0x1240;
    
    public RomBanner(BaseReader reader)
    {
        var startPosition = reader.Position;
        
        var majorVersion = reader.Read<byte>();
        var minorVersion = reader.Read<byte>();
        Version = new Version(majorVersion, minorVersion);

        BannerCrc16 = reader.Read<CRC16>();

        reader.Position += 28; // reserved

        Icon = DeserializeIcon(reader);
        Titles = new LocalizedBannerTitles(reader);
        
        reader.Position = startPosition + AnimatedIconOffset;
        AnimatedIcon = new AnimatedBannerIcon(reader);

    }
    
    private IndexedPaletteImage DeserializeIcon(BaseReader reader)
    {
        var pixels = reader.ReadPixels<Indexed4>(IconWidth, IconHeight);
        PixelSwizzler.UnSwizzle(ref pixels, IconWidth);
        var palette = new Palette("BannerPalette", reader.ReadColors<BGR555>(16));

        return new IndexedPaletteImage("BannerIcon", pixels, [palette], new ImageMetaData(IconWidth, IconHeight));
    }
}

public class LocalizedBannerTitles(BaseReader reader)
{
    public string Japanese = reader.ReadString(256, unicode: true);
    public string English = reader.ReadString(256, unicode: true);
    public string French = reader.ReadString(256, unicode: true);
    public string German = reader.ReadString(256, unicode: true);
    public string Italian = reader.ReadString(256, unicode: true);
    public string Spanish = reader.ReadString(256, unicode: true);
}

public class AnimatedBannerIcon
{
    public List<IndexedImage> Images = [];
    public List<Palette> Palettes = [];
    public List<AnimatedBannerKey> Keys = [];
    public readonly int Width = 32;
    public readonly int Height = 32;
    
    private const int ImageCount = 8;
    private const int KeyCount = 64;

    public AnimatedBannerIcon(BaseReader reader)
    {
        for (var i = 0; i < ImageCount; i++)
        {
            var pixels = reader.ReadPixels<Indexed4>(Width, Height);
            PixelSwizzler.UnSwizzle(ref pixels, Width);
            
            Images.Add(new IndexedImage($"BannerIcon_{i}", pixels, new ImageMetaData(Width, Height)));
        }
        
        for (var i = 0; i < ImageCount; i++)
        {
            var palette = new Palette($"BannerPalette_{i}", reader.ReadColors<BGR555>(16));
            
            Palettes.Add(palette.IsBlank ? Palettes[0] : palette);
        }
        
        for (var i = 0; i < KeyCount; i++)
        {
            var anim = new AnimatedBannerKey(reader);
            if (anim.IsNull) break;
            
            Keys.Add(anim);
        }
    }
}

public class AnimatedBannerKey
{
    public int Duration;
    public int BitmapIndex;
    public int PaletteIndex;
    public bool FlipHorizontal;
    public bool FlipVertical;
    
    public readonly bool IsNull;
    
    private const int TickCount = (int) (1000f / 60);
    
    public AnimatedBannerKey(BaseReader reader)
    {
        var animData = reader.Read<ushort>();
        if (animData == 0x00)
        {
            IsNull = true;
            return;
        }

        Duration = (animData & 0xFF) * TickCount;
        BitmapIndex = (animData  >> 8) & 0x3;
        PaletteIndex = (animData  >> 8) & 0x3;
        FlipHorizontal = ((animData >> 14) & 0x1) != 0;
        FlipVertical = ((animData >> 15) & 0x1) != 0;
    }
}