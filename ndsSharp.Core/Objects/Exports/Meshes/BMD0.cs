using ndsSharp.Core.Extensions;
using ndsSharp.Core.Objects.Exports.Textures;

namespace ndsSharp.Core.Objects.Exports.Meshes;

public class BMD0 : NdsObject
{
    public override string Magic => "BMD0";
    public override bool HasBlockOffsets => true;

    [Block] public MDL0 ModelData;
    [Block] public TEX0 TextureData;
}