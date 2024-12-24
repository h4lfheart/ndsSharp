using System.Globalization;
using System.Text;
using ndsSharp.Core.Data;
using ndsSharp.Core.Objects;

namespace ndsSharp.Plugins.BW2.Text;

public class BW2TextContainer : BaseDeserializable
{
    public List<string> TextEntries = [];
    
    public override void Deserialize(BaseReader reader)
    {
        var sectionCount = reader.Read<ushort>();
        var entryCount = reader.Read<ushort>();
        
        reader.Position += sizeof(uint) * 2;

        var sectionOffsets = reader.ReadArray<uint>(sectionCount);
        foreach (var sectionOffset in sectionOffsets)
        {
            reader.Position = (int) sectionOffset;
            
            var sectionSize = reader.Read<uint>();

            var cryptoState = new BW2TextCryptoState();
            for (var entryIndex = 0; entryIndex < entryCount; entryIndex++)
            {
                var entryOffset = reader.Read<uint>();
                var entrySize = reader.Read<ushort>();
                reader.Position += sizeof(ushort); // unknown
                
                cryptoState.BeginEntry();
                
                reader.Peek(() =>
                {
                    reader.Position = (int) (entryOffset + sectionOffset);

                    var entryBuilder = new StringBuilder();
                    for (var charIndex = 0; charIndex < entrySize; charIndex++)
                    {
                        entryBuilder.Append(cryptoState.GetCharacter(reader.Read<ushort>()));
                    }
                    
                    TextEntries.Add(entryBuilder.ToString());
                });
                
                cryptoState.EndEntry();
            }
        }
    }
}