using ndsSharp.Core.Extensions;
using ndsSharp.Core.Objects.Exports.Palettes.Blocks;

namespace ndsSharp.Core.Objects.Exports.Palettes;

public class NCLR : NdsObject
{
    public override string Magic => "NCLR";

    [Block] public PLTT PaletteData;
    [Block] public PCMP? CompressData;
}