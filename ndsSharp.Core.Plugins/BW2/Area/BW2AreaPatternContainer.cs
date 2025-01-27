using ndsSharp.Core.Data;
using ndsSharp.Core.Objects;
using ndsSharp.Core.Objects.Exports.Animation;
using ndsSharp.Core.Objects.Exports.Textures;

namespace ndsSharp.Core.Plugins.BW2.Area;

public class BW2AreaPatternContainer : BaseDeserializable
{
    public List<BW2PatternData> PatternDatas = [];
    public List<BTX> TextureDatas = [];
    
    public override void Deserialize(DataReader reader)
    {
        var entryCount = reader.Read<int>();
        var dataOffsets = reader.ReadArray<int>(entryCount * 2);

        for (var entryIndex = 0; entryIndex < entryCount; entryIndex++)
        {
            reader.Position = dataOffsets[entryIndex * 2];
            PatternDatas.Add(reader.ReadObject<BW2PatternData>(zeroPosition: true));
            
            reader.Position = dataOffsets[entryIndex * 2 + 1];
            TextureDatas.Add(reader.ReadObject<BTX>(zeroPosition: true));
        }
        
    }
}

public class BW2PatternData : BaseDeserializable
{
    public int FrameCount;
    public List<BW2PatternAnimation> Patterns = [];
    
    public override void Deserialize(DataReader reader)
    {
        var keyframeCount = reader.Read<int>();
        var frameIndices = reader.ReadArray<ushort>(keyframeCount);
        reader.Align(4);
        var textureIndices = reader.ReadArray<byte>(keyframeCount);
        reader.Align(4);
        var paletteIndices = reader.ReadArray<byte>(keyframeCount);
        reader.Align(4);

        var targetCount = reader.Read<int>();
        var targetIndices = reader.ReadArray<byte>(targetCount).Append((byte) keyframeCount).ToArray();
        reader.Align(4);

        FrameCount = reader.Read<int>();

        for (var targetIndex = 0; targetIndex < targetCount; targetIndex++)
        {
            var startKeyframeIndex = targetIndices[targetIndex];
            var endKeyframeIndex = targetIndices[targetIndex + 1];

            var patternAnimation = new BW2PatternAnimation();
            for (var keyframeIndex = startKeyframeIndex; keyframeIndex < endKeyframeIndex; keyframeIndex++)
            {
                patternAnimation.Keyframes.Add(new BW2PatternKeyframe
                {
                    FrameIndex = frameIndices[keyframeIndex],
                    TextureIndex = textureIndices[keyframeIndex],
                    PaletteIndex = paletteIndices[keyframeIndex],
                });
            }
            
            Patterns.Add(patternAnimation);
        }
    }
    
}

public class BW2PatternAnimation
{
    public List<BW2PatternKeyframe> Keyframes = [];
}

public class BW2PatternKeyframe
{
    public ushort FrameIndex;
    public byte TextureIndex;
    public byte PaletteIndex;
}