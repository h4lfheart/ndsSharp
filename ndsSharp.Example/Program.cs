using System.Numerics;
using ndsSharp.Core.Conversion.Models;
using ndsSharp.Core.Conversion.Models.Processing;
using ndsSharp.Core.Conversion.Textures.Images;
using ndsSharp.Core.Conversion.Textures.Screen;
using ndsSharp.Core.Objects.Exports;
using ndsSharp.Core.Objects.Exports.Meshes;
using ndsSharp.Core.Objects.Exports.Palettes;
using ndsSharp.Core.Objects.Exports.Screen;
using ndsSharp.Core.Objects.Exports.Textures;
using ndsSharp.Core.Plugins;
using ndsSharp.Core.Plugins.BW2;
using ndsSharp.Core.Plugins.BW2.Extensions;
using ndsSharp.Core.Providers;
using Serilog;
using SixLabors.ImageSharp;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var provider = new NdsFileProvider("C:/b2.nds")
{
    UnpackNARCFiles = true
};

provider.Initialize();
provider.LoadPlugins();

var palette = provider.LoadObject<NCLR>("a/2/8/7/4.nclr");
var graphic = provider.LoadObject<NCGR>("a/2/8/7/8.ncgr");
var screen = provider.LoadObject<NSCR>("a/2/8/7/12.nscr");
screen.ExtractScreenImage(graphic, palette, firstColorIsTransparent: true).ToImage().SaveAsPng($"C:/Art/{screen.File!.Name}.png");

var plugin = provider.GetPluginInterface<BW2Plugin>()!;
var matrix = plugin.GetMatrix(0);

var map = plugin.GetMap(matrix.Maps[19, 19]);
var zone = plugin.GetZone(matrix.Headers[19, 19]);
var area = plugin.GetArea(zone.AreaIndex);
var buildings = plugin.GetAreaBuildingContainer(area);

map.Model.ExtractModels().First().SaveModel("C:/Art/test.obj", MeshExportType.OBJ);

var texture = plugin.GetAreaMapTextures(area);

Log.Information("Done!");