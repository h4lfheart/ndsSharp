using ndsSharp.Core.Providers;
using ndsSharp.Plugins.BW2;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var provider = new NdsFileProvider("C:/b2.nds");
provider.UnpackNARCFiles = true;
provider.UnpackSDATFiles = true;
provider.Initialize();

var plugin = provider.GetPlugin<BW2Plugin>();
var locationTextContainer = plugin.GetSystemText(109);
foreach (var locationEntry in locationTextContainer.TextEntries)
{
    Console.WriteLine(locationEntry);
}