using System.Text.RegularExpressions;
using ndsSharp.Core.Plugins;
using ndsSharp.Plugins.BW2.Text;

namespace ndsSharp.Plugins.BW2;

public class BW2Plugin : BasePlugin
{
    public override string[] GameCodes => ["IREO", "IRBO"];

    public override PluginFileTypeAssociation[] FileTypeAssociations { get; } =
    [
        new("text", pathMatches: ["a/0/0/2", "a/0/0/3"])
    ];
}