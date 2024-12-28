using ndsSharp.Core.Conversion.Models.Mesh;
using Quaternion = OpenTK.Mathematics.Quaternion;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace ndsSharp.Viewer.Shared.Rendering.Rendering;

public class Actor(Model model, Vector3? position = null, Quaternion? rotation = null)
{
    public Model Model = model;
    public Vector3 Location = position ?? Vector3.Zero;
    public Quaternion Rotation = rotation ?? Quaternion.Identity;
}