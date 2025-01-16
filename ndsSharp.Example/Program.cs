using ndsSharp.Core.Conversion.Textures.Cells;
using ndsSharp.Core.Conversion.Textures.Images;
using ndsSharp.Core.Conversion.Textures.Pixels;
using ndsSharp.Core.Objects.Exports.Cells;
using ndsSharp.Core.Objects.Exports.Meshes;
using ndsSharp.Core.Objects.Exports.Palettes;
using ndsSharp.Core.Objects.Exports.Textures;
using ndsSharp.Core.Plugins;
using ndsSharp.Core.Plugins.BW2.Text;
using ndsSharp.Core.Plugins.HGSS.Map;
using ndsSharp.Core.Providers;
using Serilog;
using SixLabors.ImageSharp;

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

var palette = provider.LoadObject<NCLR>("a/0/3/0/4.nclr");
var texture = provider.LoadObject<NCGR>("a/0/3/0/5.ncgr");
var cell = provider.LoadObject<NCER>("a/0/3/0/2.ncer");

var cellImages = cell.ExtractCells(texture, palette);
foreach (var cellImage in cellImages)
{
    cellImage.ToImage().SaveAsPng($"C:/Art/Cells/{cellImage.Name}.png");
}