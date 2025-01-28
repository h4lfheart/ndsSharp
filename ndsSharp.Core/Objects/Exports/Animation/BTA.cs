using ndsSharp.Core.Extensions;
using ndsSharp.Core.Objects.Exports.Animation.Blocks;

namespace ndsSharp.Core.Objects.Exports.Animation;

public class BTA : NdsObject
{
    public override string Magic => "BTA";
    public override bool HasBlockOffsets => true;

    [Block] public SRT AnimationData;
}