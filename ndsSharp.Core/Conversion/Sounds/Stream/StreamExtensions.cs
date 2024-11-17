using ndsSharp.Core.Conversion.Sounds.Formats;
using ndsSharp.Core.Objects.Exports.Sounds;

namespace ndsSharp.Core.Conversion.Sounds.Stream;

public static class StreamExtensions
{
    public static Wave ToWave(this STRM strm)
    {
        var rawWaveData = strm.Data.Data.Load().GetBuffer();
        var waveData = RemapWaveData(rawWaveData, 
            strm.Header.NumChannels,
            (int) strm.Header.NumBlocks,
            (int) strm.Header.BlockLength, 
            (int) strm.Header.LastBlockLength);

        return new Wave(waveData, strm.Header.NumChannels, strm.Header.SampleRate, 16);
    }

    private static byte[] RemapWaveData(byte[] data, int numChannels, int numBlocks, int blockSize, int lastBlockSize)
    {
        var channels = new List<byte[]>();
        for (var channelIndex = 0; channelIndex < numChannels; channelIndex++)
        {
            var channelData = new List<byte>();

            for (var index = 0; index < numBlocks; index++)
            {
                var blockLength = index < numBlocks - 1 ? blockSize : lastBlockSize;
                var offset = channelIndex * blockLength;
                var blockData = new byte[blockLength];
                Array.Copy(data, index * blockLength * 2 + offset, blockData, 0, blockLength);

                channelData.AddRange(blockData);
            }
        
            channels.Add(channelData.ToArray());
        }

        var result = new List<byte>();
        for (var i = 0; i < channels[0].Length; i += 2)
        {
            foreach (var channel in channels)
            {
                result.Add(channel[i]);
                if (i + 1 < channel.Length)
                    result.Add(channel[i + 1]);
            }
        }
            
        return result.ToArray();
    }

}