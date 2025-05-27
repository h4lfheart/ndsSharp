using ndsSharp.Core.Data;
using ndsSharp.Core.Objects;

namespace ndsSharp.Core.Plugins.BW2.Map;

public class BW2MapReplaceContainer : BaseDeserializable
{
    public List<BW2MapReplacement> Replacements = [];
    
    public override void Deserialize(DataReader reader)
    {
        var fileCount = reader.Length / BW2MapReplacement.SIZE;
        for (var fileIndex = 0; fileIndex < fileCount; fileIndex++)
        {
            Replacements.Add(reader.ReadObject<BW2MapReplacement>());
        }
    }
}