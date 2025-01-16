using ndsSharp.Core.Data;

namespace ndsSharp.Core.Objects.Exports.Cells.Blocks;

public class LABL : NdsBlock
{
    public List<string> Labels = [];
    
    public override string Magic => "LABL";

    public override void Deserialize(DataReader reader)
    {
        base.Deserialize(reader);

        var labelDataOffsets = new List<uint>();

        // is this the best way to do it? there is no set label count
        while (true)
        {
            var offset = reader.Read<uint>();
            if (offset >= 0xFFFF)
            {
                reader.Position -= sizeof(uint);
                break;
            }
            
            labelDataOffsets.Add(offset);
        }

        reader.ReadWithZeroedPosition(() =>
        {
            foreach (var labelDataOffset in labelDataOffsets)
            {
                reader.Position = (int) labelDataOffset;
            
                Labels.Add(reader.ReadNullTerminatedString());
            }
        });
       
    }
}