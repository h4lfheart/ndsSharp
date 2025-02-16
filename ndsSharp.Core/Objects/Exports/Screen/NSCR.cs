using ndsSharp.Core.Extensions;
using ndsSharp.Core.Objects.Exports.Screen.Blocks;

namespace ndsSharp.Core.Objects.Exports.Screen;

public class NSCR : NdsObject
{
    public override string Magic => "NSCR";

    [Block] public SCRN ScreenData;
}