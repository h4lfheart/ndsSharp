using ndsSharp.Core.Objects.Exports.Animation;
using ndsSharp.Core.Objects.Exports.Textures;
using ndsSharp.Core.Plugins.BW2.Area;
using ndsSharp.Core.Plugins.BW2.Building;
using ndsSharp.Core.Plugins.BW2.Map;
using ndsSharp.Core.Plugins.BW2.Text;

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

    public BW2MapHeaderContainer HeaderContainer;
    private BW2AreaContainer _areaContainer;

    public override void OnLoaded()
    {
        base.OnLoaded();

        HeaderContainer = Provider.LoadObject<BW2MapHeaderContainer>("a/0/1/2/0.bin");
        _areaContainer = Provider.LoadObject<BW2AreaContainer>("a/0/1/3.bin");
    }

    public BW2MapMatrix GetMatrix(int index)
    {
        return Provider.LoadObject<BW2MapMatrix>($"a/0/0/9/{index}.matrix");
    }

    public BW2Map GetMap(int index)
    {
        return Provider.LoadObject<BW2Map>($"a/0/0/8/{index}.map");
    }

    public BW2MapHeader GetMapHeader(int index)
    {
        return HeaderContainer.Headers[index];
    }

    public BW2Area GetArea(int index)
    {
        return _areaContainer.Areas[index];
    }

    public BTX GetMapTextures(BW2Area area, ESeason season = ESeason.Spring)
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
}

public enum ESeason
{
    Spring = 0,
    Summer = 1,
    Autumn = 2,
    Winter = 3
}