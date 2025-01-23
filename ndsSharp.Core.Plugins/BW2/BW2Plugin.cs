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
        new(typeof(BW2MapMatrix), "matrix", pathMatches: ["a/0/0/9"]),
        new(typeof(BW2MapHeaderContainer), "headers", pathMatches: ["a/0/1/2"]),
        new(typeof(BW2MapBuildingContainer), "buildings", pathMatches: ["a/2/2/5", "a/2/2/6"]),
        new(typeof(BW2AreaContainer), "areas", pathMatches: ["a/0/1/3"]),
    ];

    public BW2MapHeaderContainer HeaderContainer;
    private BW2AreaContainer _areaContainer;

    public override void OnLoaded()
    {
        base.OnLoaded();

        HeaderContainer = Provider.LoadObject<BW2MapHeaderContainer>("a/0/1/2/0.headers");
        _areaContainer = Provider.LoadObject<BW2AreaContainer>("a/0/1/3.areas");
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
            $"a/0/1/4/{area.TexturesIndex + (int)(area.IsExterior ? season : ESeason.Spring)}.btx");
    }

    public BW2MapBuildingContainer GetMapBuildingContainer(BW2Area area)
    {
        var path = area.IsExterior ? "a/2/2/5" : "a/2/2/6";
        return Provider.LoadObject<BW2MapBuildingContainer>($"{path}/{area.BuildingContainerIndex}.buildings");
    }

    public BTX GetMapBuildingTextures(BW2Area area)
    {
        var path = area.IsExterior ? "a/1/7/4" : "a/1/7/5";
        return Provider.LoadObject<BTX>($"{path}/{area.BuildingContainerIndex}.btx");
    }
}

public enum ESeason
{
    Spring = 0,
    Summer = 1,
    Autumn = 2,
    Winter = 3
}