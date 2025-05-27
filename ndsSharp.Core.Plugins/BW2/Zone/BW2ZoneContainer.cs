using ndsSharp.Core.Data;
using ndsSharp.Core.Objects;

namespace ndsSharp.Core.Plugins.BW2.Zone;

public class BW2ZoneContainer : BaseDeserializable
{
    public List<BW2Zone> Headers = [];
    
    public override void Deserialize(DataReader reader)
    {
        var fileCount = reader.Length / BW2Zone.SIZE;
        for (var fileIndex = 0; fileIndex < fileCount; fileIndex++)
        {
            Headers.Add(reader.ReadObject<BW2Zone>());
        }
    }
}