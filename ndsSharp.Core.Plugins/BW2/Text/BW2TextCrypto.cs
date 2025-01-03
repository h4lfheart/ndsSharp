using System.Diagnostics;
using System.Globalization;
using ndsSharp.Core.Data;
using ndsSharp.Plugins.BW2.Text.Tokens;

namespace ndsSharp.Plugins.BW2.Text;

public class BW2TextReader
{
    private ushort _key;
    private int _entryIndex;
    
    private static Dictionary<ushort, string> _characterLUT = new()
    {
        { 0xFFFF, "\0" },
        { 0xFFFE, "\n" },
        { 0x246D, "♂" },
        { 0x246E, "♀" },
        { 0x2486, "Pk" },
        { 0x2487, "Mn" }
    };
    
    private const int StepKey = 0x2983;

    public void BeginString()
    {
        _key = (ushort)((_entryIndex + 3) * StepKey);
    }
    
    public void EndString()
    {
        _entryIndex++;
    }

    public List<BW2TextToken> GetTokens(BaseReader reader, int entrySize)
    {
        var tokens = new List<BW2TextToken>();

        var characterCounter = 0;
        while (characterCounter < entrySize)
        {
            ushort ReadCharacter()
            {
                var rawValue = reader.Read<ushort>();
            
                var value = (ushort) (rawValue ^ _key);
                StepCharacter();
                characterCounter++;

                return value;
            }
            
            var value = ReadCharacter();
            
            if (value == 0xF000) // variable identifier
            {
                var variableToken = new BW2VariableToken
                {
                    VariableType = (BW2VariableType) ReadCharacter()
                };

                var argumentCount = ReadCharacter();
                for (var argumentIndex = 0; argumentIndex < argumentCount; argumentIndex++)
                {
                    variableToken.Arguments.Add(ReadCharacter());
                }

                tokens.Add(variableToken);
            }
            else
            {
                
                var characterToken = new BW2CharacterToken
                {
                    Content = _characterLUT.TryGetValue(value, out var lutValue) ? lutValue : Convert.ToChar(value).ToString()
                };
                
                tokens.Add(characterToken);
            }
        }

        return tokens;
    }
    
    private void StepCharacter()
    {
        _key = (ushort)(((_key << 3) | (_key >> 13)) & 0xFFFF);
    }
}