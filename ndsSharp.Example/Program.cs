using ndsSharp.Core.Objects.Exports.Animation;
using ndsSharp.Core.Objects.Exports.Meshes;
using ndsSharp.Core.Providers;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var provider = new NdsFileProvider("C:/b2.nds");
provider.UnpackNARCFiles = true;
provider.UnpackSDATFiles = true;
provider.Initialize();

var model = provider.LoadObject<BMD>("a/1/5/8/2.bmd");
var animation = provider.LoadObject<BCA>("a/1/5/8/1.bca");

File.WriteAllBytes("C:/Art/Anim.bca", provider.CreateReader("a/1/5/8/1.bca").GetBuffer());
File.WriteAllBytes("C:/Art/Model.bmd", provider.CreateReader("a/1/5/8/2.bmd").GetBuffer());