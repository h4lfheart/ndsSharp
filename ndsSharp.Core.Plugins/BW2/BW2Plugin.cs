using ndsSharp.Core.Conversion.Textures.Colors;
using ndsSharp.Core.Conversion.Textures.Colors.Types;
using ndsSharp.Core.Objects.Exports.Animation;
using ndsSharp.Core.Objects.Exports.Textures;
using ndsSharp.Core.Plugins.BW2.Area;
using ndsSharp.Core.Plugins.BW2.Building;
using ndsSharp.Core.Plugins.BW2.Map;
using ndsSharp.Core.Plugins.BW2.Misc;
using ndsSharp.Core.Plugins.BW2.Text;
using ndsSharp.Core.Plugins.BW2.Zone;

namespace ndsSharp.Core.Plugins.BW2;

public class BW2Plugin : BasePlugin
{
    public override string[] GameCodes => ["IREO", "IRBO"];

    public override PluginFileTypeAssociation[] FileTypeAssociations { get; } =
    [
        new(typeof(BW2Text), "text", pathMatches: ["a/0/0/2", "a/0/0/3"]),
        new(typeof(BW2Map), "map", pathMatches: ["a/0/0/8"]),
        new(typeof(BW2MapMatrix), "matrix", pathMatches: ["a/0/0/9"])
    ];

    public BW2ZoneContainer ZoneContainer;
    private BW2AreaContainer _areaContainer;
    private BW2NPCRegistry _npcRegistry;
    private BW2MapReplaceContainer _replaceContainer;

    public override void OnLoaded()
    {
        base.OnLoaded();

        ZoneContainer = Provider.LoadObject<BW2ZoneContainer>("a/0/1/2/0.bin");
        _areaContainer = Provider.LoadObject<BW2AreaContainer>("a/0/1/3.bin");
        _npcRegistry = Provider.LoadObject<BW2NPCRegistry>("a/0/4/7/0.bin");

        _replaceContainer = Provider.LoadObject<BW2MapReplaceContainer>("a/0/1/0/0.bin");
    }

    public BW2MapMatrix GetMatrix(int index)
    {
        return Provider.LoadObject<BW2MapMatrix>($"a/0/0/9/{index}.matrix");
    }

    public BW2Map GetMap(int index)
    {
        return Provider.LoadObject<BW2Map>($"a/0/0/8/{index}.map");
    }
    
    public BW2NPC GetNPC(int index)
    {
        return _npcRegistry.NPCs[index];
    }

    
    public BW2Zone GetZone(int index)
    {
        return ZoneContainer.Headers[index];
    }
    
    public BW2ZoneEntityContainer GetZoneEntities(int index)
    {
        return Provider.LoadObject<BW2ZoneEntityContainer>($"a/1/2/6/{index}.bin");
    }

    public BW2Area GetArea(int index)
    {
        return _areaContainer.Areas[index];
    }

    public BTX GetAreaMapTextures(BW2Area area, ESeason season = ESeason.Spring)
    {
        return Provider.LoadObject<BTX>(
            $"a/0/1/4/{area.TexturesIndex + (int) (area.IsExterior ? season : ESeason.Spring)}.btx");
    }

    public BW2MapBuildingContainer GetAreaBuildingContainer(BW2Area area)
    {
        var path = area.IsExterior ? "a/2/2/5" : "a/2/2/6";
        return Provider.LoadObject<BW2MapBuildingContainer>($"{path}/{area.BuildingContainerIndex}.bin");
    }

    public BTX GetAreaBuildingTextures(BW2Area area)
    {
        var path = area.IsExterior ? "a/1/7/4" : "a/1/7/5";
        return Provider.LoadObject<BTX>($"{path}/{area.BuildingContainerIndex}.btx");
    }
    
    public BW2AreaPatternContainer? GetAreaPatternAnimation(BW2Area area)
    {
        Provider.TryLoadObject<BW2AreaPatternContainer>($"a/0/6/9/{area.PatternAnimIndex}.bin", out var pattern);
        return pattern;
    }
    
    public BTA? GetAreaMaterialAnimation(BW2Area area)
    {
        Provider.TryLoadObject<BTA>($"a/0/6/8/{area.MaterialAnimIndex}.bta", out var bta);
        return bta;
    }
    
    public BW2AreaLightingContainer? GetAreaLightingContainer(BW2Area area)
    {
        Provider.TryLoadObject<BW2AreaLightingContainer>($"a/0/6/0/{area.LightIndex}.bin", out var lightingContainer);
        return lightingContainer;
    }

    public Color[] GetAreaOutlineColors(BW2Area area)
    {
        return Provider.TryCreateReader($"a/0/1/5/{area.OutlineIndex}.bin", out var reader) ? reader.ReadColors<BGR555>(8) : [];
    }
    
    public int GetOverrideMatrixIndex(ushort matrixIndex, EGameVersion gameVersion, ESeason season)
    {
        var replacementData = _replaceContainer.Replacements
            .FirstOrDefault(replacement => replacement.MatrixIndex == matrixIndex && replacement.IsMatrixReplacement);
        
        return replacementData?.Condition switch
        {
            BW2MapReplacement.CONDITION_SEASON => replacementData.Values[(int) season],
            BW2MapReplacement.CONDITION_VERSION => replacementData.Values[(int) gameVersion],
            _ => matrixIndex
        };
    }

    public int GetOverrideMapIndex(ushort matrixIndex, ushort mapIndex, EGameVersion gameVersion, ESeason season)
    {
        var replacementData = _replaceContainer.Replacements
            .Where(replacement => replacement.MatrixIndex == matrixIndex && !replacement.IsMatrixReplacement)
            .FirstOrDefault(replacement => replacement.Values[0] == mapIndex);
        
        return replacementData?.Condition switch
        {
            BW2MapReplacement.CONDITION_SEASON => replacementData.Values[(int) season],
            BW2MapReplacement.CONDITION_VERSION => replacementData.Values[(int) gameVersion],
            _ => mapIndex
        };
    }
}

public enum EGameVersion
{
    Black2,
    White2
}

public enum ESeason
{
    Spring = 0,
    Summer = 1,
    Autumn = 2,
    Winter = 3
}