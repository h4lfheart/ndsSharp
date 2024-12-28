using System.Diagnostics;
using System.Reflection;
using ndsSharp.Core.Extensions;
using ndsSharp.Core.Plugins;
using ndsSharp.Core.Providers;
using Serilog;

namespace ndsSharp.Plugins;

public static class PluginExtensions
{
    public static void LoadPlugins(this NdsFileProvider provider)
    {
        var pluginTypes = Assembly.GetExecutingAssembly().DefinedTypes.Where(type => type.IsAssignableTo(typeof(BasePlugin)));
        foreach (var pluginType in pluginTypes)
        {
            if (Activator.CreateInstance(pluginType) is not BasePlugin pluginInstance) continue;
            if (!pluginInstance.GameCodes.Contains(provider.Header.GameCode)) continue;

            pluginInstance.Provider = provider;

            foreach (var file in provider.Files.Values.ToArray())
            {
                foreach (var fileTypeAssociation in pluginInstance.FileTypeAssociations)
                {
                    if (!fileTypeAssociation.Match(file)) continue;
                    
                    provider.Files.Remove(file.Path);

                    var newPath = file.Path.SubstringBeforeWithLast('.') + fileTypeAssociation.Extension;
                    file.Path = newPath;
                    provider.Files[newPath] = file;
                    
                    break;
                }
            }
          
            provider.Plugins[pluginType] = pluginInstance;
            pluginInstance.OnLoaded();
        }
    }
}