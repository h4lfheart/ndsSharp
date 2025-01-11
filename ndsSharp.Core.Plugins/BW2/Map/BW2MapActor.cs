using System.Numerics;
using ndsSharp.Core.Data;
using ndsSharp.Core.Objects;

namespace ndsSharp.Plugins.BW2.Map;

public class BW2MapActor : BaseDeserializable
{
    public Vector3 Location = Vector3.Zero;
    public float Rotation;
    public int ModelID;
    
    public override void Deserialize(DataReader reader)
    {
        Location.X = reader.Read<int>() / 4096f;
        Location.Y = reader.Read<int>() / 4096f;
        Location.Z = reader.Read<int>() / 4096f;
        Rotation = reader.Read<ushort>() * 360f / 65536f;
        ModelID = (reader.Read<byte>() << 8) + reader.Read<byte>();
    }
}