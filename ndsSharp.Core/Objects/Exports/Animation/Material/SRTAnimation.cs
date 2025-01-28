using System.Diagnostics;
using ndsSharp.Core.Data;
using ndsSharp.Core.Data.Reader;
using ndsSharp.Core.Extensions;

namespace ndsSharp.Core.Objects.Exports.Animation.Material;

public class SRTAnimation : DeserializableWithName
{
    public List<SRTTrack> Tracks = [];
    public ushort FrameCount;
    public string ReadMagic;
    
    public override void Deserialize(DataReader reader)
    {
        ReadMagic = reader.ReadString(4);
        FrameCount = reader.Read<ushort>();

        reader.Position += sizeof(ushort);

        var trackList = reader.ReadNameList(() => reader.ReadArray(5, () => reader.ReadObject<SRTChannelInfo>()));
        foreach (var (name, channelInfos) in trackList.Items)
        {
            var track = new SRTTrack
            {
                Name = name
            };

            for (var channelIndex = 0; channelIndex < channelInfos.Length; channelIndex++)
            {
                var channelType = (SRTChannelType) channelIndex;
                var channelInfo = channelInfos[channelIndex];

                if (channelType == SRTChannelType.Rotation)
                {
                    track.Channels[channelType] = new SRTChannel
                    {
                        FrameCount = channelInfo.FrameCount,
                        Samples = ReadRotationSamples(reader, channelInfo)
                    };
                }
                else
                {
                    track.Channels[channelType] = new SRTChannel
                    {
                        FrameCount = channelInfo.FrameCount,
                        Samples = ReadFloatSamples(reader, channelInfo)
                    };
                }
            }
            
            Tracks.Add(track);
        }
    }

    public float[] ReadFloatSamples(DataReader reader, SRTChannelInfo channelInfo)
    {
        var isShortData = channelInfo.Flags.Bit(12) == 1;
        var isConstant = channelInfo.Flags.Bit(13) == 1;
        
        float[] samples;
        if (isConstant) // const
        {
            samples = [channelInfo.OffsetOrConst / 4096f];
        }
        else // offset
        {
            reader.Position = (int) channelInfo.OffsetOrConst;

            if (isShortData)
            {
                samples = reader
                    .ReadArray<short>(channelInfo.FrameCount)
                    .Select(value => value / 4096f)
                    .ToArray();
            }
            else
            {
                samples = reader
                    .ReadArray<int>(channelInfo.FrameCount)
                    .Select(value => value / 4096f)
                    .ToArray();
            }
            
        }
        

        return samples;
    }
    
    public float[] ReadRotationSamples(DataReader reader, SRTChannelInfo channelInfo)
    {
        var isConstant = channelInfo.Flags.Bit(13) == 1;
        
        float[] samples;
        if (isConstant) // const
        {
            samples = [MathF.Atan2((channelInfo.OffsetOrConst & 0xFFFF) / 4096f, (channelInfo.OffsetOrConst >> 16 & 0xFFFF) / 4096f)];
        }
        else // offset
        {
            reader.Position = (int) channelInfo.OffsetOrConst;

            samples = reader
                .ReadArray(channelInfo.FrameCount, () => (reader.Read<short>(), reader.Read<short>()))
                .Select(values => MathF.Atan2(values.Item1 / 4096f, values.Item2 / 4096f))
                .ToArray();
        }
        
        return samples;
    }
}
