namespace ndsSharp.Plugins.BW2.Text.Tokens;

public class BW2CharacterToken : BW2TextToken
{
    public string Content;

    public override string ToString()
    {
        return Content;
    }
}