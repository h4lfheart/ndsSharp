using System.Numerics;
using ndsSharp.Core.Data;
using ndsSharp.Core.Extensions;
using ndsSharp.Core.Mathematics;

namespace ndsSharp.Core.Objects.Exports.Meshes.Model;

public class MDLTransform : DeserializableWithName
{
    public Vector3 Translation = Vector3.Zero;
    public Matrix3x3 Rotation;
    public Vector3 Scale = Vector3.One;
    public Matrix4x4 Matrix = Matrix4x4.Identity;
    
    public override void Deserialize(BaseReader reader)
    {
        var flag = reader.Read<ushort>();

        var isTranslate = (flag & 1) == 0;
        var isRotate = (flag >> 1 & 1) == 0;
        var isScale = (flag >> 2 & 1) == 0;
        var isPivot = (flag >> 3 & 1) == 1;

        var m0 = reader.ReadShortAsFloat();

        if (isTranslate)
        {
            Translation = new Vector3(reader.ReadIntAsFloat(), reader.ReadIntAsFloat(), reader.ReadIntAsFloat());
        }

        if (isPivot)
        {
            var matrixSelection = flag >> 4 & 0b1111;
            var negateFlag = flag >> 8 & 0b111;
            var a = reader.ReadShortAsFloat();
            var b = reader.ReadShortAsFloat();
            Rotation = Matrix3x3.CreatePivot(matrixSelection, negateFlag, a, b);
        }
        else if (isRotate)
        {
            Rotation = new Matrix3x3(
                m0, reader.ReadShortAsFloat(), reader.ReadShortAsFloat(),
                reader.ReadShortAsFloat(), reader.ReadShortAsFloat(), reader.ReadShortAsFloat(),
                reader.ReadShortAsFloat(), reader.ReadShortAsFloat(), reader.ReadShortAsFloat());
        }

        if (isScale)
        {
            Scale = new Vector3(reader.ReadIntAsFloat(), reader.ReadIntAsFloat(), reader.ReadIntAsFloat());
            reader.Position += sizeof(int) * 3; // inverse scale
        }
        
        // create final matrix
        if (isScale)
        {
            Matrix = Matrix4x4.CreateScale(Scale);
        }
        if (isRotate || isPivot)
        {
            Matrix *= Rotation.To4x4();
        }
        if (isTranslate)
        {
            Matrix *= Matrix4x4.CreateTranslation(Translation);
        }
        
    }
    
}