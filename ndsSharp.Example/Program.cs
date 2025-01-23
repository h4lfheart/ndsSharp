using System.Diagnostics;
using ndsSharp.Core.Conversion.Textures.Cells;
using ndsSharp.Core.Conversion.Textures.Images;
using ndsSharp.Core.Conversion.Textures.Pixels;
using ndsSharp.Core.Objects.Exports.Archive;
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
    UnpackNARCFiles = true
};

provider.Initialize();
provider.LoadPlugins();

var compressedFiles = provider.Files.Where(x => x.Value.Compression is not null).ToArray();
Log.Information("There are {Length} total compressed files", compressedFiles.Length);
Log.Information("Took {TotalSeconds}s to check for these patterns", NARC.CompressTimer.Elapsed.TotalSeconds);

var compressedTexture = provider.LoadObject<NCGR>("a/0/5/1/0.ncgr");
var palette = provider.LoadObject<NCLR>("a/0/5/1/18.nclr");

compressedTexture.CombineWith(palette, true).ToImage().SaveAsPng("C:/Art/Cells/Base.png");

var cells = provider.LoadObject<NCER>("a/0/5/1/4.ncer");
cells.ExtractCells(compressedTexture, palette).ForEach(image => image.ToImage().SaveAsPng($"C:/Art/Cells/{image.Name}.png"));

Debugger.Break();