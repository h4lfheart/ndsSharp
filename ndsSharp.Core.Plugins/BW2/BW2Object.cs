using ndsSharp.Core.Data;
using ndsSharp.Core.Objects;

namespace ndsSharp.Core.Plugins.BW2;

public class BW2Object : BaseDeserializable
{
    public string Magic;
    public int[] FileOffsets = [];
    
    public override void Deserialize(DataReader reader)
    {
        Magic = reader.ReadString(2);

        var fileCount = reader.Read<ushort>();
        FileOffsets = reader.ReadArray<int>(fileCount);
    }
}