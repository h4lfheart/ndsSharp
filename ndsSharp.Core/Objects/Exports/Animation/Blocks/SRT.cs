using ndsSharp.Core.Data;
using ndsSharp.Core.Objects.Exports.Animation.Material;

namespace ndsSharp.Core.Objects.Exports.Animation.Blocks;

public class SRT : NdsBlock
{
    public List<SRTAnimation> Animations = [];
    
    public override string Magic => "SRT";

    public override void Deserialize(DataReader reader)
    {
        base.Deserialize(reader);

        var animationList = reader.ReadNameListPrimitive<int>();
        foreach (var (name, offset) in animationList.Items)
        {
            reader.Position = offset;
            
            var animation = reader.ReadObject<SRTAnimation>(zeroPosition: true, dataModifier: mat => mat.Name = name);
            Animations.Add(animation);
        }
    }
}