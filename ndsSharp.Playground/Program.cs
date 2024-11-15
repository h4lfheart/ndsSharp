using ndsSharp.Core;
using ndsSharp.Core.Conversion.Models;
using ndsSharp.Core.Conversion.Models.Export;
using ndsSharp.Core.Conversion.Models.Processing;
using ndsSharp.Core.Conversion.Textures.Images;
using ndsSharp.Core.Extensions;
using ndsSharp.Core.Objects.Exports;
using ndsSharp.Core.Objects.Exports.Archive;
using ndsSharp.Core.Objects.Exports.Meshes;
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

provider.LogFileStats();

var meshFiles = provider.GetAllFilesOfType<BMD0>().ToArray();
var count = 0;
foreach (var meshFile in meshFiles)
{
    Log.Information("Mesh {Name}: {First} of {Second}", meshFile.Path, count + 1, meshFiles.Length);
    var mesh = provider.LoadObject<BMD0>(meshFile);
    
    var models = mesh.ExtractModels();
    foreach (var model in models)
    {
        model.SaveToDirectory($"C:/Art/Models/{meshFile.Path.SubstringBeforeLast("/")}", MeshExportType.OBJ);
    }

    count++;
}
