using ndsSharp.Core.Plugins;
using ndsSharp.Core.Plugins.HGSS.Map;
using ndsSharp.Core.Providers;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var provider = new NdsFileProvider("C:/hgss.nds")
{
    UnpackNARCFiles = true,
    UnpackSDATFiles = true
};

provider.Initialize();
provider.LoadPlugins();

var matrix = provider.LoadObject<HGSSMapMatrix>("a/0/4/1/0.matrix");
Log.Information("ae");