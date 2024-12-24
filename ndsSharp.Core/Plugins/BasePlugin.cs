using ndsSharp.Core.Providers;

namespace ndsSharp.Core.Plugins;

public abstract class BasePlugin
{
    public NdsFileProvider Owner;
    public abstract string[] GameCodes { get; }
    
    public virtual void OnLoaded() { }
}