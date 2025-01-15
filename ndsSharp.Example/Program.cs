using ndsSharp.Core.Objects.Exports.Meshes;
using ndsSharp.Core.Objects.Exports.Palettes;
using ndsSharp.Core.Objects.Exports.Textures;
using ndsSharp.Core.Plugins;
using ndsSharp.Core.Plugins.BW2.Text;
using ndsSharp.Core.Plugins.HGSS.Map;
using ndsSharp.Core.Providers;
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

var paletteFiles = provider.GetAllFilesOfType<NCLR>().ToArray();
foreach (var paletteFile in paletteFiles)
{
    var palette = provider.LoadObject<NCLR>(paletteFile);
}

var characterFiles = provider.GetAllFilesOfType<NCGR>().ToArray();
foreach (var characterFile in characterFiles)
{
    var character = provider.LoadObject<NCGR>(characterFile);
}
