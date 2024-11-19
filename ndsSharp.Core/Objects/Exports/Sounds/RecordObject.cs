using ndsSharp.Core.Data;
using ndsSharp.Core.Objects.Files;

namespace ndsSharp.Core.Objects.Exports.Sounds;

public abstract class RecordObject<TInfoType> : NdsObject where TInfoType : BaseSoundInfo
{
    public TInfoType Info;
    public int RecordId;

    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);

        if (Owner is not SDATRomFile sdatRomFile) return;
        
        Info = (TInfoType) sdatRomFile.SoundInfo;
        RecordId = sdatRomFile.RecordId;
    }
}

public abstract class RecordObject : RecordObject<BaseSoundInfo>;