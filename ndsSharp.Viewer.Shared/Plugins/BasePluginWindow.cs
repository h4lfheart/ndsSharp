using ndsSharp.Core.Providers;
using ndsSharp.Viewer.Shared.Framework;

namespace ndsSharp.Viewer.Shared.Plugins;

public class BasePluginWindow<T>() : WindowBase<T>(initializeWindowModel: false) where T : BasePluginWindowModel, new()
{
    
}