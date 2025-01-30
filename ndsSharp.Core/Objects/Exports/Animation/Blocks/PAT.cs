using ndsSharp.Core.Data;
using ndsSharp.Core.Objects.Exports.Animation.Pattern;

namespace ndsSharp.Core.Objects.Exports.Animation.Blocks;

public class PAT : NdsBlock
{
    public List<PATAnimation> Animations = [];
    
    public override string Magic => "PAT";
    
    public override void Deserialize(DataReader reader)
    {
        base.Deserialize(reader);
        
        var animationList = reader.ReadNameListPrimitive<int>();
        foreach (var (name, offset) in animationList.Items)
        {
            reader.Position = offset;
            
            var animation = reader.ReadObject<PATAnimation>(zeroPosition: true, dataModifier: pattern => pattern.Name = name);
            Animations.Add(animation);
        }
    }
}