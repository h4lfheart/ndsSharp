using ndsSharp.Core.Conversion.Textures.Images;
using ndsSharp.Core.Objects.Exports.Textures;
using ndsSharp.Core.Providers;
using ndsSharp.Plugins;
using Serilog;
using SixLabors.ImageSharp;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var provider = new NdsFileProvider("C:/b2.nds");
provider.UnpackNARCFiles = true;
provider.UnpackSDATFiles = true;
provider.Initialize();
provider.LoadPlugins();

var textureFile = provider.LoadObject<BTX>("a/0/1/4/0.btx");
foreach (var image in textureFile.TextureData.Textures)
{
    image.ToImage().SaveAsPng("");
}
