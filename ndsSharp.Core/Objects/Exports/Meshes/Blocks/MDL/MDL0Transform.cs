using System.Numerics;
using ndsSharp.Core.Data;
using ndsSharp.Core.Mathematics;

namespace ndsSharp.Core.Objects.Exports.Meshes.Blocks.MDL;

public class MDL0Transform : DeserializableWithName
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
                
            var o = (negateFlag >> 0 & 1) == 0 ? 1 : -1;
            var c = (negateFlag >> 1 & 1) == 0 ? b : -b;
            var d = (negateFlag >> 2 & 1) == 0 ? a : -a;

            Rotation = matrixSelection switch
            {
                0 => new Matrix3x3(
                    o, 0, 0, 
                    0, a, b, 
                    0, c, d),
                1 => new Matrix3x3(
                    0, o, 0, 
                    a, 0, b, 
                    c, 0, d),
                2 => new Matrix3x3(
                    0, 0, o, 
                    a, b, 0, 
                    c, d, 0),
                3 => new Matrix3x3(
                    0, a, b, 
                    o, 0, 0, 
                    0, c, d),
                4 => new Matrix3x3(
                    a, 0, b, 
                    0, o, 0, 
                    c, 0, d),
                5 => new Matrix3x3(
                    a, b, 0, 
                    0, 0, o, 
                    c, d, 0),
                6 => new Matrix3x3(
                    0, a, b,
                    0, c, d, 
                    o, 0, 0),
                7 => new Matrix3x3(
                    a, 0, b, 
                    c, 0, d, 
                    0, o, 0),
                8 => new Matrix3x3(
                    a, b, 0, 
                    c, d, 0, 
                    0, 0, o)
            };
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