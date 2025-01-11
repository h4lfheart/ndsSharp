using ndsSharp.Core.Data;
using ndsSharp.Core.Data.Checksum;
using Serilog;

namespace ndsSharp.Core.Objects.Rom;

public class RomHeader
{
    public string Title;
    public string GameCode;
    public string MakerCode;
    public UnitCode UnitCode;
    public byte EncryptionSeedSelect;
    public long CartridgeSize;
    public byte RomVersion;
    public bool AutoStart;

    public ArmMeta Arm9Offset;
    public ArmMeta Arm7Offset;

    public DataPointer FntPointer;
    public DataPointer FatPointer;
    public DataPointer Arm9Overlay;
    public DataPointer Arm7Overlay;

    public uint BannerOffset;
    
    public CRC16 SecureCrc16;
    public ushort ReadTimeout;

    public uint RomSize;
    public uint HeaderSize;

    public byte[] NintendoLogo;
    public CRC16 LogoCrc16;
    public CRC16 HeaderCrc16;
    
    private const int BaseCartridgeSize = 1280000;
    
    public RomHeader(DataReader reader)
    {
        Title = reader.ReadString(12);
        GameCode = reader.ReadString(4);
        MakerCode = reader.ReadString(2);
        UnitCode = reader.ReadEnum<UnitCode>();
        EncryptionSeedSelect = reader.Read<byte>();

        var deviceCapacity = reader.Read<byte>();
        CartridgeSize = BaseCartridgeSize * (1 << deviceCapacity);
        
        reader.Position += 9; // reserved

        RomVersion = reader.Read<byte>();
        AutoStart = (reader.Read<byte>() & 2) == 1;

        Arm9Offset = new ArmMeta(reader);
        Arm7Offset = new ArmMeta(reader);

        FntPointer = new DataPointer(reader);
        FatPointer = new DataPointer(reader);

        Arm9Overlay = new DataPointer(reader);
        Arm7Overlay = new DataPointer(reader);

        reader.Position += sizeof(uint) * 2; // port settings

        BannerOffset = reader.Read<uint>();
        
        SecureCrc16 = reader.Read<CRC16>();
        ReadTimeout = reader.Read<ushort>();

        reader.Position += sizeof(uint) * 2; // auto load ram addresses
        reader.Position += sizeof(uint) * 2; // secure area disable

        RomSize = reader.Read<uint>();
        HeaderSize = reader.Read<uint>();

        reader.Position += sizeof(uint) * 14; // reserved

        NintendoLogo = reader.ReadArray<byte>(156);
        LogoCrc16 = reader.Read<CRC16>();

        HeaderCrc16 = reader.Read<CRC16>();

    }
}

public class ArmMeta
{
    public int Offset;
    public int ExecuteAddress;
    public int Destination;
    public int Size;
    
    public ArmMeta(DataReader reader)
    {
        Offset = reader.Read<int>();
        ExecuteAddress = reader.Read<int>();
        Destination = reader.Read<int>();
        Size = reader.Read<int>();
    }
}


public enum UnitCode : byte
{
    DS = 0,
    Both = 2,
    DSi = 3
}
