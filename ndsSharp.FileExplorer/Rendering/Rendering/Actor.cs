using System.Numerics;
using ndsSharp.Core.Conversion.Models.Mesh;
using Quaternion = OpenTK.Mathematics.Quaternion;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace ndsSharp.FileExplorer.Rendering.Rendering;

public class Actor(Model model, Vector3? position = null, Quaternion? rotation = null)
{
    public Model Model = model;
    public Vector3 Position = position ?? Vector3.Zero;
    public Quaternion Rotation = rotation ?? Quaternion.Identity;
}