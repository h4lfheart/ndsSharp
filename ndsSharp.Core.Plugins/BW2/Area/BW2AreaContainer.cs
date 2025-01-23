using ndsSharp.Core.Data;
using ndsSharp.Core.Objects;

namespace ndsSharp.Core.Plugins.BW2.Area;

public class BW2AreaContainer : BaseDeserializable
{
    public List<BW2Area> Areas = [];
    
    public override void Deserialize(DataReader reader)
    {
        var fileCount = reader.Length / BW2Area.SIZE;
        for (var fileIndex = 0; fileIndex < fileCount; fileIndex++)
        {
            Areas.Add(reader.ReadObject<BW2Area>());
        }
    }
}