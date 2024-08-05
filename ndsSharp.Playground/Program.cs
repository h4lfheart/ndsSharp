using ndsSharp.Core;
using ndsSharp.Core.Objects.Exports;
using ndsSharp.Core.Objects.Exports.Archive;
using ndsSharp.Core.Providers;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var provider = new NdsFileProvider("C:/b2.nds");
provider.UnpackNARCFiles = true;
provider.Initialize();

Log.Information($"{provider.Files.Count} Files Loaded");

var testFile = provider.LoadObject<NdsObject>("skb/0.bin");
foreach (var block in testFile.Blocks)
{
    Log.Information($"File has block {block.ReadMagic}");
}