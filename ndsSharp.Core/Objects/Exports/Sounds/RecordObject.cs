using ndsSharp.Core.Data;
using ndsSharp.Core.Objects.Files;

namespace ndsSharp.Core.Objects.Exports.Sounds;

public abstract class RecordObject<TInfoType> : NdsObject where TInfoType : BaseSoundInfo
{
    public TInfoType Info;
    public int RecordId;

    public override void Deserialize(DataReader reader)
    {
        base.Deserialize(reader);

        if (File is not SDATRomFile sdatRomFile) return;
        
        Info = (TInfoType) sdatRomFile.SoundInfo;
        RecordId = sdatRomFile.RecordId;
    }
}

public abstract class RecordObject : RecordObject<BaseSoundInfo>;