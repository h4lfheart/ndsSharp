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
using ndsSharp.Core.Objects.Files;
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

var bankFiles = provider.GetAllFilesOfType<SBNK>().ToArray();
var waveArchiveFiles = provider.GetAllFilesOfType<SWAR>().ToArray();
foreach (var bankFile in bankFiles)
{
    var bank = provider.LoadObject<SBNK>(bankFile);
    if (bank.ParentSDAT is null) continue;
    
    Log.Information("Bank: {Name}", bank.Owner.Path);
    for (var index = 0; index < bank.Data.Instruments.Count; index++)
    {
        var instrument = bank.Data.Instruments[index];
        Log.Information(" Instrument {Index}: {Type}", index, instrument.Type.ToString());
        switch (instrument.Type)
        {
            case InstrumentType.PCM:
            case InstrumentType.PSG:
            case InstrumentType.Noise:
            {
                var data = instrument.GetData<DefaultInstrumentData>();
                var waveArchiveId = bank.Info.WaveArchiveIDs[data.Info.ArchiveIndex];
                var waveFile = waveArchiveFiles.FirstOrDefault(file =>
                {
                    if (file is not SDATRomFile sdatRomFile) return false;
                    
                    return sdatRomFile.ParentSDAT.Equals(bank.ParentSDAT) &&
                           sdatRomFile.RecordId == waveArchiveId;
                });

                if (waveFile is null)
                {
                    Log.Information("    {Type} Sample Invalid", instrument.Type.ToString());
                    break;
                }
                
                Log.Information("    {Type} Sample {WaveArchiveName}[{Index}]", instrument.Type.ToString(), waveFile.Name, data.Info.WaveIndex);
                break;
            }
            case InstrumentType.Drum:
            {
                var data = instrument.GetData<DrumInstrumentData>();
                Log.Information("    Min Note: {Min}", data.MinNote);
                Log.Information("    Max Note: {Max}", data.MaxNote);
                var noteIndex = data.MinNote;
                foreach (var subInstrument in data.Instruments)
                {
                    var waveArchiveId = bank.Info.WaveArchiveIDs[subInstrument.Info.ArchiveIndex];
                    var waveFile = waveArchiveFiles.FirstOrDefault(file =>
                    {
                        if (file is not SDATRomFile sdatRomFile) return false;
                    
                        return sdatRomFile.ParentSDAT.Equals(bank.ParentSDAT) &&
                               sdatRomFile.RecordId == waveArchiveId;
                    });

                    if (waveFile is null)
                    {
                        Log.Information("    {Type} Sample: Invalid - Note {NoteIndex}", subInstrument.Type.ToString(), noteIndex);
                        noteIndex++;
                        continue;
                    }
                
                    Log.Information("    {Type} Sample: {WaveArchiveName}[{Index}] - Note {NoteIndex}", subInstrument.Type.ToString(), waveFile.Name, subInstrument.Info.WaveIndex, noteIndex);
                    noteIndex++;
                }
                break;
            }
            case InstrumentType.KeyRegion:
            {
                var data = instrument.GetData<KeyRegionInstrumentData>();

                var regionIndex = 0;
                foreach (var subInstrument in data.Instruments)
                {
                    var regionString = regionIndex switch
                    {
                        0 => $"0 - {data.KeyRegions[regionIndex]}",
                        > 0 => $"{data.KeyRegions[regionIndex - 1]} - {data.KeyRegions[regionIndex]}"
                    };
                    
                    var waveArchiveId = bank.Info.WaveArchiveIDs[subInstrument.Info.ArchiveIndex];
                    var waveFile = waveArchiveFiles.FirstOrDefault(file =>
                    {
                        if (file is not SDATRomFile sdatRomFile) return false;
                    
                        return sdatRomFile.ParentSDAT.Equals(bank.ParentSDAT) &&
                               sdatRomFile.RecordId == waveArchiveId;
                    });

                    if (waveFile is null)
                    {
                        Log.Information("    {Type} Sample: Invalid - Region {RegionString}", subInstrument.Type.ToString(), regionString);
                        regionIndex++;
                        continue;
                    }
                
                    Log.Information("    {Type} Sample: {WaveArchiveName}[{Index}] - Region {RegionString}", subInstrument.Type.ToString(), waveFile.Name, subInstrument.Info.WaveIndex, regionString);
                    regionIndex++;
                }
                break;
            }
        }
    }
}