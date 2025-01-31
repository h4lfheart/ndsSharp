using ndsSharp.Core.Data;
using ndsSharp.Core.Objects;

namespace ndsSharp.Core.Plugins.BW2.Area;

public class BW2AreaLightingContainer : BaseDeserializable
{
    public BW2AreaLighting[] LightingDatas = [];
    
    public override void Deserialize(DataReader reader)
    {
        var fileCount = reader.Length / BW2AreaLighting.SIZE;
        LightingDatas = reader.ReadArray(fileCount, () => reader.ReadObject<BW2AreaLighting>());
    }
}