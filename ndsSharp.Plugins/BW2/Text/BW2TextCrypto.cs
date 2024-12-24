using System.Diagnostics;
using System.Globalization;

namespace ndsSharp.Plugins.BW2.Text;

public class BW2TextCryptoState
{
    private ushort Key;
    private int EntryIndex;
    private bool AwaitingControlCode;

    private static Dictionary<ushort, string> CharacterLUT = new()
    {
        { 0xFFFF, "\0" },
        { 0xFFFE, "\n" },
        { 0x246D, "♂" },
        { 0x246E, "♀" },
        { 0x2486, "Pk" },
        { 0x2487, "Mn" }
    };
    
    private const int STEP_KEY = 0x2983;

    public void BeginEntry()
    {
        Key = (ushort)((EntryIndex + 3) * STEP_KEY);
    }
    
    public void EndEntry()
    {
        EntryIndex++;
    }

    public string GetCharacter(ushort rawValue)
    {
        var correctedValue = (ushort) (rawValue ^ Key);
        StepCharacter();

        if (AwaitingControlCode)
        {
            AwaitingControlCode = false;
            
            // TODO create custom class for the character-by-character handling instead of putting this as text
            var enumValue = (BW2ControlType) correctedValue;
            return Enum.IsDefined(typeof(BW2ControlType), enumValue) ? $"<{enumValue}>" : "\0";
        }

        if (correctedValue == 0xF000) // variable
        {
            AwaitingControlCode = true;
            return "\0";
        }
        
        if (CharacterLUT.TryGetValue(correctedValue, out var lutValue))
        {
            return lutValue;
        }

        var correctedCharacter = Convert.ToChar(correctedValue);
        
        return correctedCharacter.ToString();
    }
    
    private void StepCharacter()
    {
        Key = (ushort)(((Key << 3) | (Key >> 13)) & 0xFFFF);
    }
}

public enum BW2ControlType : ushort
{
    // TODO add the rest of the control types!!
    
    SCROLL = 0xBE00,
    CLEAR = 0xBE01,
    WAIT = 0xBE02,
    SPEED_UP = 0xBE09,
    BLANK = 0xBDFF,
    COLOR = 0xFF00,
    
    TRAINER_NAME = 0x0100,
    POKEMON_NAME = 0x0101,
    POKEMON_NICKNAME = 0x0102,
    TYPE = 0x0103,
    LOCATION = 0x0105,
    ABILITY = 0x0106,
    MOVE = 0x0107,
    ITEM1 = 0x0108,
    ITEM2 = 0x0109,
}