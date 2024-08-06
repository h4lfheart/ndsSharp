using ndsSharp.Conversion.Textures;
using ndsSharp.Core;
using ndsSharp.Core.Objects.Exports;
using ndsSharp.Core.Objects.Exports.Archive;
using ndsSharp.Core.Objects.Exports.Textures;
using ndsSharp.Core.Providers;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var provider = new NdsFileProvider("C:/b2.nds");
provider.UnpackNARCFiles = true;
provider.Initialize();

Log.Information($"{provider.Files.Count} Files Loaded");

var textureFiles = provider.GetAllFilesOfType<BTX0>();

foreach (var textureFile in textureFiles)
{
    Log.Information($"Exporting {textureFile.Path}");
    
    var path = $"C:/Art/B2/{textureFile.Path}";
    Directory.CreateDirectory(path);
    var textureContainer = provider.LoadObject<BTX0>(textureFile);

    var exporter = new TextureExporter(textureContainer.TextureData);
    exporter.Export(path);
}