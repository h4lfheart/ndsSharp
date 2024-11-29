using ndsSharp.Core.Extensions;
using ndsSharp.Core.Objects.Exports.Animation.Blocks;

namespace ndsSharp.Core.Objects.Exports.Animation;

public class BCA : NdsObject
{
    public override string Magic => "BCA";
    public override bool HasBlockOffsets => true;
    
    [Block] public JNT AnimationData;
}