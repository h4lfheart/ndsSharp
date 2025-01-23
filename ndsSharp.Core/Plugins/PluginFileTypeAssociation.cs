using System.Text.RegularExpressions;
using ndsSharp.Core.Objects.Files;

namespace ndsSharp.Core.Plugins;

public class PluginFileTypeAssociation(Type fileType, string extension, string[]? pathMatches = null, Regex[]? regexMatches = null, string[]? magicMatches = null)
{
    public Type FileType = fileType;
    public string Extension = extension;
    public string[] PathMatches = pathMatches ?? [];
    public Regex[] RegexMatches = regexMatches ?? [];
    public string[] MagicMatches = magicMatches ?? [];

    public bool Match(RomFile file)
    {
        var matched = false;
        matched |= PathMatches.Any(match => file.Path.Contains(match));
        matched |= RegexMatches.Any(regex => regex.IsMatch(file.Path));
        matched |= MagicMatches.Any(magic =>
        {
            throw new NotImplementedException();
        });
        return matched;
    }
}