using ndsSharp.Core.Objects.Exports.Sounds;
using ndsSharp.Core.Providers;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var provider = new NdsFileProvider("C:/b2.nds");
provider.UnpackNARCFiles = true;
provider.UnpackSDATFiles = true;
provider.Initialize();

provider.LogFileStats();

var sseq = provider.LoadObject<SSEQ>("swan_sound_data/sseq/seq_bgm_vs_achroma.sseq");