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
                var channelType = channelIndex switch
                {
                    3 => SRTChannelType.TranslationU,
                    4 => SRTChannelType.TranslationV,
                    _ => SRTChannelType.None
                };

                var channelInfo = channelInfos[channelIndex];

                if (channelType == SRTChannelType.None)
                {
                    track.Channels[channelIndex] = new SRTChannel
                    {
                        FrameCount = channelInfo.FrameCount,
                        ChannelType = channelType
                    };
                    continue;
                }
                
                reader.Position = (int) channelInfo.Offset;

                var channel = new SRTChannel
                {
                    FrameCount = channelInfo.FrameCount,
                    ChannelType = channelType,
                    Samples = reader
                        .ReadArray<ushort>(channelInfo.FrameCount)
                        .Select(value => FloatExtensions.ToFloat(value, 1, 10, 5))
                        .ToArray()
                };

                track.Channels[channelIndex] = channel;
            }
            
            Tracks.Add(track);
        }
    }
}
