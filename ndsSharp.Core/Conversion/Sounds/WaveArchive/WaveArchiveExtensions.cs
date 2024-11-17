using System.Diagnostics;
using ndsSharp.Core.Conversion.Sounds.Decoding;
using ndsSharp.Core.Conversion.Sounds.Formats;
using ndsSharp.Core.Extensions;
using ndsSharp.Core.Objects.Exports.Sounds;

namespace ndsSharp.Core.Conversion.Sounds.WaveArchive;

public static class WaveArchiveExtensions
{
    public static List<Wave> ExtractWaves(this SWAR swar)
    {
        var waves = new List<Wave>();
        for (var waveIndex = 0; waveIndex < swar.Data.NumWaves; waveIndex++)
        {
            var waveInfo = swar.Data.WaveInfos[waveIndex];
            var rawWaveData = swar.Data.WavePointers[waveIndex].Load().GetBuffer();
            var waveData = waveInfo.WaveType switch
            {
                WaveType.PCM8 => PCM.PCM8ToPCM16(rawWaveData),
                WaveType.PCM16 => rawWaveData,
                WaveType.ADPCM => ADPCM.Decode(rawWaveData, rawWaveData.Length * 2)
            };
            
            
            waves.Add(new Wave(waveData, 1, waveInfo.SampleRate, 16));
        }
        

        return waves;
    }
}