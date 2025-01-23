using ndsSharp.Core.Data;

namespace ndsSharp.Core.Objects.Exports.Cells.Blocks;

public class UEXT : NdsBlock
{
    public override string Magic => "UEXT";

    public override void Deserialize(DataReader reader)
    {
        base.Deserialize(reader);

        reader.Position += sizeof(uint); // bro what is this
    }
}