using ndsSharp.Core;
using ndsSharp.Core.Conversion.Models;
using ndsSharp.Core.Conversion.Models.Export;
using ndsSharp.Core.Conversion.Models.Processing;
using ndsSharp.Core.Conversion.Sounds.Stream;
using ndsSharp.Core.Conversion.Sounds.WaveArchive;
using ndsSharp.Core.Conversion.Textures.Images;
using ndsSharp.Core.Extensions;
using ndsSharp.Core.Objects.Exports;
using ndsSharp.Core.Objects.Exports.Archive;
using ndsSharp.Core.Objects.Exports.Meshes;
using ndsSharp.Core.Objects.Exports.Sounds;
using ndsSharp.Core.Objects.Exports.Textures;
using ndsSharp.Core.Providers;
using Serilog;
using SixLabors.ImageSharp;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var provider = new NdsFileProvider("C:/b2.nds");
provider.UnpackNARCFiles = true;
provider.UnpackSDATFiles = true;
provider.Initialize();

provider.LogFileStats();

var waveArchiveFiles = provider.GetAllFilesOfType<SWAR>();
foreach (var waveArchiveFile in waveArchiveFiles)
{
    var swar = provider.LoadObject<SWAR>(waveArchiveFile);
    var swarName = waveArchiveFile.Name.SubstringBeforeLast(".");
    var waves = swar.ExtractWaves();
    for (var i = 0; i < waves.Count; i++)
    {
        var waveName = waves.Count > 1 ? $"{swarName}_{i}" : swarName;
        File.WriteAllBytes($"C:/Art/Waves/{waveName}.wav", waves[i].GetBuffer());
    }
}

var streamFiles = provider.GetAllFilesOfType<STRM>();
foreach (var streamFile in streamFiles)
{
    var strm = provider.LoadObject<STRM>(streamFile);
    var strmName = streamFile.Name.SubstringBeforeLast(".");
    var wave = strm.ToWave();
    
    File.WriteAllBytes($"C:/Art/{strmName}.wav", wave.GetBuffer());
}