using ndsSharp.Core.Conversion.Textures.Cells;
using ndsSharp.Core.Conversion.Textures.Images;
using ndsSharp.Core.Conversion.Textures.Pixels;
using ndsSharp.Core.Objects.Exports.Cells;
using ndsSharp.Core.Objects.Exports.Palettes;
using ndsSharp.Core.Objects.Exports.Textures;
using ndsSharp.Core.Plugins;
using ndsSharp.Core.Providers;
using Serilog;
using SixLabors.ImageSharp;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var provider = new NdsFileProvider("C:/b2.nds")
{
    CacheObjects = true,
    UnpackNARCFiles = true
};

provider.Initialize();
provider.LoadPlugins();

var palette = provider.LoadObject<NCLR>("a/2/8/7/14.nclr");
var graphic = provider.LoadObject<NCGR>("a/2/8/7/16.ncgr");
var cell = provider.LoadObject<NCER>("a/2/8/7/17.ncer");

graphic.CombineWith(palette).ToImage().SaveAsPng("C:/Art/Cell.png");

var cells = cell.ExtractCells(graphic, palette);
cells.ForEach(cell => cell.ToImage().SaveAsPng($"C:/Art/Cells/{cell.Name}.png"));

Log.Information("Done!");