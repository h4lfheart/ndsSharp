using ndsSharp.Core;
using ndsSharp.Core.Conversion.Textures.Images;
using ndsSharp.Core.Objects.Exports;
using ndsSharp.Core.Objects.Exports.Archive;
using ndsSharp.Core.Objects.Exports.Textures;
using ndsSharp.Core.Providers;
using Serilog;
using SixLabors.ImageSharp;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var provider = new NdsFileProvider("C:/b2.nds");
provider.UnpackNARCFiles = true;
provider.Initialize();

Log.Information($"{provider.Files.Count} Files Loaded");

provider.Banner.Icon.ToImageSharp().SaveAsPng("C:/Art/B2.png");
provider.Banner.AnimatedIcon.ToImageSharp().SaveAsPng("C:/Art/B2_Anim.png");