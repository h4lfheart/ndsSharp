using ndsSharp.Core.Data;
using ndsSharp.Core.Objects.Exports;
using ndsSharp.Core.Objects.Exports.Sounds;

namespace ndsSharp.Core.Objects.Files;

public class SDATRomFile(string path, DataPointer pointer, BaseSoundInfo soundInfo, int recordId, RomFile? owner = null) : RomFile(path, pointer, owner)
{
    public BaseSoundInfo? SoundInfo = soundInfo;
    public int RecordId = recordId;
}