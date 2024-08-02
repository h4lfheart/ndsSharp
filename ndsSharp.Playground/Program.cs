using System.Diagnostics;
using ndsSharp.Core;
using Newtonsoft.Json;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var provider = new NdsFileProvider("C:/b2.nds");
provider.Initialize();

Debug.Assert(provider.Header.LogoCrc16.Matches(provider.Header.NintendoLogo));

Log.Information(JsonConvert.SerializeObject(provider.Header, Formatting.Indented));

Log.Information($"{provider.Files.Count} Files Loaded");