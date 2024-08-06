using ndsSharp.Core.Data;
using ndsSharp.Core.Extensions;

namespace ndsSharp.Core.Objects.Exports.Textures;

public class BTX0 : NdsObject
{
    public override string Magic => "BTX0";
    public override bool HasBlockOffsets => true;

    [Block] public TEX0 TextureData;
}