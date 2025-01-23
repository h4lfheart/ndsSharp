using ndsSharp.Core.Data;

namespace ndsSharp.Core.Objects.Exports.Archive.Blocks;

public class FIMG : NdsBlock
{
    public DataReader Reader;
    
    public override string Magic => "FIMG";

    public override void Deserialize(DataReader reader)
    {
        base.Deserialize(reader);

        var pointer = new DataPointer((int) DataOffset, (int) DataSize);
        Reader = reader.LoadPointer(pointer);
    }
}