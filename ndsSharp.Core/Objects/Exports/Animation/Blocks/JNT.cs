using ndsSharp.Core.Data;
using ndsSharp.Core.Objects.Exports.Animation.Anim;

namespace ndsSharp.Core.Objects.Exports.Animation.Blocks;

public class JNT : NdsBlock
{
    public List<JNTAnimation> Animations = [];
    
    public override string Magic => "JNT";

    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);

        var animationList = reader.ReadNameListPrimitive<uint>();
        foreach (var (name, offset) in animationList.Items)
        {
            reader.Position = (int) offset;

            var animation = reader.ReadObject<JNTAnimation>(dataModifier: anim => anim.Name = name, zeroPosition: true);
            Animations.Add(animation);
        }
    }
}