using ndsSharp.Core.Plugins;
using ndsSharp.Plugins.BW2.Text;
using Serilog;

namespace ndsSharp.Plugins.BW2;

public class BW2Plugin : BasePlugin
{
    public override string[] GameCodes => ["IREO", "IRBO"];

    private const string SystemTextFolder = "a/0/0/2";
    private const string GameTextFolder = "a/0/0/3";

    public override void OnLoaded()
    {
        // anything that needs to be loaded before plugin init happens here
    }

    public BW2TextContainer GetSystemText(int index)
    {
        return Owner.LoadObject<BW2TextContainer>($"{SystemTextFolder}/{index}.bin");
    }
    
    public BW2TextContainer GetGameText(int index)
    {
        return Owner.LoadObject<BW2TextContainer>($"{GameTextFolder}/{index}.bin");
    }
}