using ndsSharp.Core.Data;

namespace ndsSharp.Core.Objects.Exports;

public class NdsBlock : BaseDeserializable
{
    public string ReadMagic;
    public uint FileSize;

    public long DataOffset => HEADER_SIZE;
    public long DataSize => FileSize - HEADER_SIZE;

    public NdsObject Owner;
    
    public virtual string Magic => string.Empty;

    private const int HEADER_SIZE = 8; // 4 char magic + file size
    
    public override void Deserialize(DataReader reader)
    {
        ReadMagic = reader.ReadString(4, Owner.IsLittleEndian).Trim().TrimEnd('0');
        if (!string.IsNullOrEmpty(Magic) && ReadMagic != Magic)
        {
            throw new ParserException($"{Magic} has invalid magic: {ReadMagic}");
        }

        FileSize = reader.Read<uint>();
    }
}