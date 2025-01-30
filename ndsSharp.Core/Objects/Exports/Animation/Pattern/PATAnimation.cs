using ndsSharp.Core.Data;

namespace ndsSharp.Core.Objects.Exports.Animation.Pattern;

public class PATAnimation : DeserializableWithName
{
    public List<PATTrack> Tracks = [];
    
    public ushort FrameCount;
    public string[] TextureNames = [];
    public string[] PaletteNames = [];

    public string ReadMagic;

    public override void Deserialize(DataReader reader)
    {
        ReadMagic = reader.ReadString(4);
        
        FrameCount = reader.Read<ushort>();

        var textureNameCount = reader.Read<byte>();
        var paletteNameCount = reader.Read<byte>();
        var textureNameOffset = reader.Read<ushort>();
        var paletteNameOffset = reader.Read<ushort>();

        var trackList = reader.ReadNameList<PATTrackInfo>();
        foreach (var (name, trackInfo) in trackList.Items)
        {
            reader.Position = trackInfo.Offset;
            
            Tracks.Add(new PATTrack
            {
                Name = name,
                KeyframeCount = trackInfo.KeyframeCount,
                Keyframes = reader.ReadArray((int) trackInfo.KeyframeCount, () => reader.ReadObject<PATKeyframe>())
            });
        }

        reader.Position = textureNameOffset;
        TextureNames = reader.ReadArray(textureNameCount, () => reader.ReadString(16));
        
        reader.Position = paletteNameOffset;
        PaletteNames = reader.ReadArray(paletteNameCount, () => reader.ReadString(16));
    }
}