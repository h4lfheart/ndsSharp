using ndsSharp.Core.Providers;
using ndsSharp.Plugins;
using ndsSharp.Plugins.BW2;
using ndsSharp.Plugins.BW2.Text;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var provider = new NdsFileProvider("C:/b2.nds");
provider.UnpackNARCFiles = true;
provider.UnpackSDATFiles = true;
provider.Initialize();
provider.LoadPlugins();

var locationTextContainer = provider.LoadObject<BW2Text>("a/0/0/2.text");
foreach (var locationEntry in locationTextContainer.TextEntries)
{
    Console.WriteLine(locationEntry);
}