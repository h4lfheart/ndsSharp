using ndsSharp.Core.Data;

namespace ndsSharp.Core.Objects.Rom;

public class RomBanner
{
    public Version Version;

    public ushort BannerCrc16;
    
    
    public RomBanner(BaseReader reader)
    {
        var majorVersion = reader.Read<byte>();
        var minorVersion = reader.Read<byte>();
        Version = new Version(majorVersion, minorVersion);

        BannerCrc16 = reader.Read<ushort>();

        reader.Position += 28; // reserved
        
        
    }
}
