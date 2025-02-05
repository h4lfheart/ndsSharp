using System.Numerics;
using ndsSharp.Core.Conversion.Models;
using ndsSharp.Core.Conversion.Models.Processing;
using ndsSharp.Core.Objects.Exports;
using ndsSharp.Core.Objects.Exports.Meshes;
using ndsSharp.Core.Objects.Exports.Textures;
using ndsSharp.Core.Plugins;
using ndsSharp.Core.Plugins.BW2;
using ndsSharp.Core.Plugins.BW2.Extensions;
using ndsSharp.Core.Providers;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var provider = new NdsFileProvider("C:/b2.nds")
{
    UnpackNARCFiles = true
};

provider.Initialize();
provider.LoadPlugins();

var plugin = provider.GetPluginInterface<BW2Plugin>()!;
var matrix = plugin.GetMatrix(0);

var map = plugin.GetMap(matrix.Maps[18, 23]);
map.Terrain?.ExtractTerrainModel().SaveModel("C:/Art/terrain.obj", MeshExportType.OBJ);
Log.Information("Done!");