using System.Numerics;
using ndsSharp.Core.Conversion.Models;
using ndsSharp.Core.Conversion.Models.Processing;
using ndsSharp.Core.Plugins;
using ndsSharp.Core.Plugins.BW2;
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

var headerIndex = matrix.HasHeaders ? matrix.Headers[23, 18] : -1;
var header = headerIndex != -1 
    ? plugin.GetMapHeader(headerIndex) 
    : plugin.HeaderContainer.Headers.FirstOrDefault(header => header.MatrixIndex == 0);
if (header is null) return;

var area = plugin.GetArea(header.AreaIndex);

var buildingContainer = plugin.GetAreaBuildingContainer(area);
var buildingTextures = plugin.GetAreaBuildingTextures(area);
var materialAnimation = plugin.GetAreaMaterialAnimation(area);
var lightingContainer = plugin.GetAreaLightingContainer(area);
Log.Information("Done!");