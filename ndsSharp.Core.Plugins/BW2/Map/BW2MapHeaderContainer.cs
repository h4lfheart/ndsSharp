using ndsSharp.Core.Data;
using ndsSharp.Core.Objects;

namespace ndsSharp.Plugins.BW2.Map;

public class BW2MapHeaderContainer : BaseDeserializable
{
    public List<BW2MapHeader> Headers = [];
    
    public override void Deserialize(BaseReader reader)
    {
        var fileCount = reader.Length / BW2MapHeader.HEADER_SIZE;
        for (var fileIndex = 0; fileIndex < fileCount; fileIndex++)
        {
            Headers.Add(reader.ReadObject<BW2MapHeader>());
        }
    }
}