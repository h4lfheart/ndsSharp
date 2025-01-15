using ndsSharp.Core.Extensions;
using ndsSharp.Core.Objects.Exports.Textures.Blocks;

namespace ndsSharp.Core.Objects.Exports.Textures;

public class NCGR : NdsObject
{
    public override string Magic => "NCGR";

    [Block] public CHAR CharacterData;
    [Block] public CPOS PositionData;
}