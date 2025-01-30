using ndsSharp.Core.Extensions;
using ndsSharp.Core.Objects.Exports.Animation.Blocks;

namespace ndsSharp.Core.Objects.Exports.Animation;

public class BTP : NdsObject
{
    public override string Magic => "BTP";
    public override bool HasBlockOffsets => true;

    [Block] public PAT AnimationData;
}