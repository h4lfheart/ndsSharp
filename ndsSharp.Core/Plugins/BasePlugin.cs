using ndsSharp.Core.Providers;

namespace ndsSharp.Core.Plugins;

public abstract class BasePlugin
{
    public NdsFileProvider Provider;
    public abstract string[] GameCodes { get; }
    public virtual PluginFileTypeAssociation[] FileTypeAssociations { get; }
    
    public virtual void OnLoaded() { }
}