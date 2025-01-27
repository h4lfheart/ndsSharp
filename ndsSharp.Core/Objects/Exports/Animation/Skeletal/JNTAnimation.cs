using ndsSharp.Core.Data;
using ndsSharp.Core.Extensions;
using ndsSharp.Core.Mathematics;
using ndsSharp.Core.Objects.Exports.Meshes.Model;

namespace ndsSharp.Core.Objects.Exports.Animation.Anim;

public class JNTAnimation : DeserializableWithName
{
    public List<JNTTrack> Tracks = [];
    
    public ushort NumFrames;
    public ushort NumTracks;

    public uint PivotMatricesOffset;
    public uint BasisMatricesOffset;

    public ushort[] TrackOffsets = [];
    
    public override void Deserialize(DataReader reader)
    {
        reader.Position += 4; // identifier? always J \0 A C

        NumFrames = reader.Read<ushort>();
        NumTracks = reader.Read<ushort>();

        reader.Position += sizeof(uint);
        
        PivotMatricesOffset = reader.Read<uint>();
        BasisMatricesOffset = reader.Read<uint>();

        TrackOffsets = reader.ReadArray<ushort>(NumTracks);

        foreach (var trackOffset in TrackOffsets)
        {
            reader.Position = trackOffset;

            var track = reader.ReadObject<JNTTrack>(track => track.Owner = this);
            Tracks.Add(track);
        }
    }
}