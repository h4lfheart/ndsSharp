using ndsSharp.Core.Extensions;
using ndsSharp.Core.Objects.Exports.Cells.Blocks;

namespace ndsSharp.Core.Objects.Exports.Cells;

public class NCER : NdsObject
{
    public override string Magic => "NCER";

    [Block] public CEBK CellData;
    [Block] public LABL LabelData;
    [Block] public UEXT ExtraData;
}