using ndsSharp.Core.Data;
using ndsSharp.Core.Objects.Files;

namespace ndsSharp.Core.Objects.Exports.Archive;

public class FIMG : NdsBlock
{
    public BaseReader Reader;
    
    public override string Magic => "FIMG";

    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);

        var pointer = new DataPointer((int) DataOffset, (int) DataSize);
        Reader = reader.LoadPointer(pointer);
    }
}