using System.Text;
using ndsSharp.Core.Plugins.BW2.Text.Tokens;

namespace ndsSharp.Core.Plugins.BW2.Text;

public class BW2TextString
{
    public List<BW2TextToken> Tokens = [];

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        Tokens.ForEach(token => stringBuilder.Append(token));
        return stringBuilder.ToString();
    }
}