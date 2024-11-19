using ndsSharp.Core.Data;
using ndsSharp.Core.Objects.Exports.Sounds;

namespace ndsSharp.Core.Objects.Files;

public class SDATRomFile(string path, DataPointer pointer, SDAT parentSdat, BaseSoundInfo soundInfo, int recordId) : RomFile(path, pointer)
{
    public SDAT? ParentSDAT = parentSdat;
    public BaseSoundInfo? SoundInfo = soundInfo;
    public int RecordId = recordId;
}