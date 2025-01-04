using ndsSharp.Core.Objects.Exports.Meshes;
using ndsSharp.Core.Providers;
using ndsSharp.Plugins;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var provider = new NdsFileProvider("C:/b2.nds")
{
    UnpackNARCFiles = true,
    UnpackSDATFiles = true
};

provider.Initialize();
provider.LoadPlugins();

var bmd = provider.LoadObject<BMD>("a/0/1/1/0.bmd");