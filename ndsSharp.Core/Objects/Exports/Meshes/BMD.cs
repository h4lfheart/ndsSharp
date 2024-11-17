using ndsSharp.Core.Extensions;
using ndsSharp.Core.Objects.Exports.Meshes.Blocks;
using ndsSharp.Core.Objects.Exports.Textures;
using ndsSharp.Core.Objects.Exports.Textures.Blocks;

namespace ndsSharp.Core.Objects.Exports.Meshes;

public class BMD : NdsObject
{
    public override string Magic => "BMD";
    public override bool HasBlockOffsets => true;

    [Block] public MDL ModelData;
    [Block] public TEX TextureData;
}