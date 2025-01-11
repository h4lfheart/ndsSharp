using System.Globalization;
using System.Text;
using ndsSharp.Core.Data;
using ndsSharp.Core.Objects;

namespace ndsSharp.Plugins.BW2.Text;

public class BW2Text : BaseDeserializable
{
    public List<BW2TextSection> Sections = [];
    
    public override void Deserialize(DataReader reader)
    {
        var sectionCount = reader.Read<ushort>();
        var entryCount = reader.Read<ushort>();
        
        reader.Position += sizeof(uint) * 2;

        var sectionOffsets = reader.ReadArray<uint>(sectionCount);
        foreach (var sectionOffset in sectionOffsets)
        {
            reader.Position = (int) sectionOffset;
            
            var sectionSize = reader.Read<uint>();

            var textReader = new BW2TextReader();
            var section = new BW2TextSection();
            for (var entryIndex = 0; entryIndex < entryCount; entryIndex++)
            {
                var entryOffset = reader.Read<uint>();
                var entrySize = reader.Read<ushort>();
                reader.Position += sizeof(ushort); // unknown
                
                var textString = new BW2TextString();
                textReader.BeginString();
                
                reader.Peek(() =>
                {
                    reader.Position = (int) (entryOffset + sectionOffset);

                    textString.Tokens = textReader.GetTokens(reader, entrySize);
                });
                
                textReader.EndString();
                
                section.Strings.Add(textString);
            }
            
            Sections.Add(section);
        }
    }
}

public class BW2TextSection 
{
    public List<BW2TextString> Strings = [];
}