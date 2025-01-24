using ndsSharp.Core.Conversion.Textures.Cells;
using ndsSharp.Core.Conversion.Textures.Images;
using ndsSharp.Core.Objects.Exports.Cells;
using ndsSharp.Core.Objects.Exports.Palettes;
using ndsSharp.Core.Objects.Exports.Textures;
using ndsSharp.Core.Plugins;
using ndsSharp.Core.Providers;
using Serilog;
using SixLabors.ImageSharp;

namespace ndsSharp.Example;

public static class Program
{
    public static void Main()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        var provider = new NdsFileProvider("C:/b2.nds")
        {
            UnpackNARCFiles = true
        };

        provider.Initialize();
        provider.LoadPlugins();

        var ncgr = provider.LoadObject<NCGR>("a/0/3/0/5.ncgr");
        var nclr = provider.LoadObject<NCLR>("a/0/3/0/4.nclr");
        var ncer = provider.LoadObject<NCER>("a/0/3/0/0.ncer");

        foreach (var indexedPaletteImage in ncer.ExtractCells(ncgr, nclr))
        {
            indexedPaletteImage.ToImage().SaveAsPng($"C:/Art/Cells/{indexedPaletteImage.Name}.png");
        }
    }
}