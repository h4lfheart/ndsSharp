namespace ndsSharp.Plugins.BW2.Text.Tokens;

public class BW2VariableToken : BW2TextToken
{
    public BW2VariableType VariableType;
    public List<ushort> Arguments = [];

    public override string ToString()
    {
        return $"<{VariableType}>";
    }
}

public enum BW2VariableType : ushort
{
    Scroll = 0xBE00,
    Clear = 0xBE01,
    Wait = 0xBE02,
    SpeedUp = 0xBE09,
    Blank = 0xBDFF,
    Color = 0xFF00,
    
    ColorEx = 0xBD00,
    ColorReset = 0xBD01,
    AlignCenter = 0xBD02,
    AlignRight = 0xBD03,
    SkipPixels = 0xBD04,
    SetXPosition = 0xBD05,
    
    TrainerName = 0x0100,
    PokemonName = 0x0101,
    PokemonNickname = 0x0102,
    Type = 0x0103,
    Location = 0x0105,
    Ability = 0x0106,
    Move = 0x0107,
    Item1 = 0x0108,
    Item2 = 0x0109,
    MusicalProp = 0x010A,
    Box = 0x010B,
    BattlePokemon = 0x010C,
    Stat = 0x010D,
    TrainerClass = 0x010E,
    Hobby = 0x010F,
    PassPower = 0x0110,
    Bag = 0x0112,
    SurveyResult = 0x0113,
    
    Generic = 0x011C,
    
    MusicalName = 0x0122,
    MusicalFeeling = 0x0123,
    
    Country = 0x0124,
    Province = 0x0125,
    
    MusicalBodyPart = 0x0131,
    DecorationName = 0x0132,
    MusicalAudience = 0x0133,
    Medal = 0x0135,
    MedalRank = 0x0136,
    JoinAvenueInput = 0x0137,
    
    Tournament = 0x013B,
    BattleMode = 0x013C,
    Title = 0x013D,
    Weather = 0x013E,
    MovieName = 0x013F,
    FunfestMission = 0x0140,
    JoinAvenueRank = 0x0142,
    EntralinkLevel = 0x0143,
    
    NumberDigits1 = 0x0200,
    NumberDigits2 = 0x0201,
    NumberDigits3 = 0x0202,
    NumberDigits4 = 0x0203,
    NumberDigits5 = 0x0204,
    NumberDigits6 = 0x0205,
    NumberDigits7 = 0x0206,
    NumberDigits8 = 0x0207,
    NumberDigits9 = 0x0208,
}