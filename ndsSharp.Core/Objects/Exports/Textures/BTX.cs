using ndsSharp.Core.Data;
using ndsSharp.Core.Extensions;
using ndsSharp.Core.Objects.Exports.Textures.Blocks;

namespace ndsSharp.Core.Objects.Exports.Textures;

public class BTX : NdsObject
{
    public override string Magic => "BTX";
    public override bool HasBlockOffsets => true;

    [Block] public TEX TextureData;
}