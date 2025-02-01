using System.Numerics;
using ndsSharp.Core.Conversion.Models;
using ndsSharp.Core.Conversion.Models.Processing;
using ndsSharp.Core.Objects.Exports;
using ndsSharp.Core.Objects.Exports.Meshes;
using ndsSharp.Core.Objects.Exports.Textures;
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

var map = plugin.GetMap(matrix.Maps[23, 18]);
var models = map.Model.ExtractModels();

var headerIndex = matrix.HasHeaders ? matrix.Headers[23, 18] : -1;
var header = headerIndex != -1 
    ? plugin.GetZone(headerIndex) 
    : plugin.ZoneContainer.Headers.FirstOrDefault(header => header.MatrixIndex == 0);
if (header is null) return;

var entityContainer = plugin.GetZoneEntities(header.EntityIndex);
var entityResources = provider.Files.Where(kvp => kvp.Key.StartsWith("a/0/4/8/")).Select(kvp => kvp.Value).ToArray();
foreach (var zoneNpc in entityContainer.NPCs)
{
    var npcDefinition = plugin.GetNPC(zoneNpc.ObjectCode);
    var resourceFile = entityResources[npcDefinition.ResourceIndices[0]];
    var resource = (NdsObject) provider.LoadObject(resourceFile, resourceFile.FileType);
}
Log.Information("Done!");