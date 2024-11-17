using System.Diagnostics;
using System.Reflection;
using ndsSharp.Core.Data;
using ndsSharp.Core.Extensions;
using ndsSharp.Core.Objects.Exports.Sounds;
using ndsSharp.Core.Objects.Files;
using Serilog;

namespace ndsSharp.Core.Objects.Exports;

public class NdsObject : BaseDeserializable
{
    public string ReadMagic;
    public ushort BOM;
    public Version Version;
    public uint FileSize;
    public ushort BlockStart;
    public ushort BlockCount;
    
    public List<NdsBlock> Blocks = [];

    public bool IsBigEndian => BOM == BIG_ENDIAN_BOM;
    public bool IsLittleEndian => BOM == LITTLE_ENDIAN_BOM;
    
    public virtual string Magic => string.Empty;
    public virtual bool HasBlockOffsets => false;
    
    private const int LITTLE_ENDIAN_BOM = 0xFFFE;
    private const int BIG_ENDIAN_BOM = 0xFEFF;

    public override void Deserialize(BaseReader reader)
    {
        ReadMagic = reader.ReadString(4).TrimEnd('0');
        if (!string.IsNullOrEmpty(Magic) &&  ReadMagic != Magic)
        {
            throw new ParserException($"{Magic} has invalid magic: {ReadMagic}");
        }

        BOM = reader.Read<ushort>();
        
        var version = reader.Read<ushort>();
        Version = IsBigEndian ? new Version(version & 0xFF,  version >> 8) : new Version( version >> 8, version & 0xFF);
        
        FileSize = reader.Read<uint>();
        BlockStart = reader.Read<ushort>();
        BlockCount = reader.Read<ushort>();
        
        if (HasBlockOffsets)
        {
            var blockOffsets = reader.ReadArray(BlockCount, reader.Read<uint>);
            foreach (var offset in blockOffsets)
            {
                reader.Position = (int) offset;
                
                var (extension, length) = reader.Peek(() => (reader.ReadString(4, IsLittleEndian).TrimEnd('0'), reader.Read<uint>()));
                var pointer = new DataPointer((int) offset, (int) length);
                if (FileTypeRegistry.TryGetType(extension, GetType(), out var type))
                {
                    Blocks.Add(reader.LoadPointer(pointer).ReadObject<NdsBlock>(type, block => block.Owner = this));
                }
                else
                {
                    Blocks.Add(reader.LoadPointer(pointer).ReadObject<NdsBlock>(block => block.Owner = this));
                    Log.Warning($"Unknown Block {extension}");
                }
            }
        }
        else
        {
            reader.Position = BlockStart;
            for (var blockIndex = 0; blockIndex < BlockCount; blockIndex++)
            {
                var offset = reader.Position;
            
                var (extension, length) = reader.Peek(() => (reader.ReadString(4, IsLittleEndian).Trim().TrimEnd('0'), reader.Read<uint>()));
                var pointer = new DataPointer(offset, (int) length);
                if (FileTypeRegistry.TryGetType(extension, GetType(), out var type))
                {
                    Blocks.Add(reader.LoadPointer(pointer).ReadObject<NdsBlock>(type, block => block.Owner = this));
                }
                else
                {
                    Blocks.Add(reader.LoadPointer(pointer).ReadObject<NdsBlock>(block => block.Owner = this));
                    Log.Warning($"Unknown Block {extension}");
                }
            
                reader.Position = (int) (offset + length);
            }
        }
        
        var fields = GetType().GetFields();
        foreach (var field in fields)
        {
            var blockAttribute = field.GetCustomAttribute<BlockAttribute>();
            if (blockAttribute is null) continue;
            
            field.SetValue(this, Blocks.FirstOrDefault(block => block.GetType() == field.FieldType));
        }
    }
}