using System.Numerics;
using ndsSharp.Core.Extensions;

namespace ndsSharp.Core.Mathematics;

public struct Matrix3x3(
    float m11, float m12, float m13,
    float m21, float m22, float m23,
    float m31, float m32, float m33)
{
    public static readonly Matrix3x3 Zero = new();
    public static readonly Matrix3x3 Identity = new()
    {
        M11 = 1.0f, M21 = 0.0f, M31 = 0.0f,
        M12 = 0.0f, M22 = 1.0f, M32 = 0.0f,
        M13 = 0.0f, M23 = 0.0f, M33 = 1.0f
    };

    public float M11 = m11;
    public float M12 = m12;
    public float M13 = m13;
    public float M21 = m21;
    public float M22 = m22;
    public float M23 = m23;
    public float M31 = m31;
    public float M32 = m32;
    public float M33 = m33;

    public static Matrix3x3 CreatePivot(int matrixSelection, int negateFlag, float a, float b)
    {
        var o = (negateFlag >> 0 & 1) == 0 ? 1 : -1;
        var c = (negateFlag >> 1 & 1) == 0 ? b : -b;
        var d = (negateFlag >> 2 & 1) == 0 ? a : -a;

        return matrixSelection switch
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

    // don't ask me how, nsbmd model format docs carried this
    public static Matrix3x3 CreateBasis(ushort[] rawInputData)
    {
        ushort[] remappedInputData = [rawInputData[4], ..rawInputData[..4]];

        var rawOutputData = new ushort[6];
        for (var i = 0; i < 5; i++)
        {
            var remappedData = remappedInputData[i];
            rawOutputData[5] = (ushort) ((rawOutputData[5] << 3) | remappedData.Bits(0, 3));
            rawOutputData[i] = remappedInputData[i].Bits(3, 16);
        }

        var floatOutputData = rawOutputData.Select(data => FloatExtensions.ToFloat(data, 1, 0, 12)).ToArray();
        var a = new Vector3(floatOutputData[1], floatOutputData[2], floatOutputData[3]);
        var b = new Vector3(floatOutputData[4], floatOutputData[0], floatOutputData[5]);
        var c = Vector3.Cross(a, b);

        return new Matrix3x3
        {
            M11 = a.X, M21 = b.X, M31 = c.X,
            M12 = a.Y, M22 = b.Y, M32 = c.Y,
            M13 = a.Z, M23 = b.Z, M33 = c.Z
        };
    }

    public Matrix4x4 To4x4()
    {
        var matrix = Matrix4x4.Identity;
        matrix.M11 = M11;
        matrix.M12 = M12;
        matrix.M13 = M13;
        matrix.M21 = M21;
        matrix.M22 = M22;
        matrix.M23 = M23;
        matrix.M31 = M31;
        matrix.M32 = M32;
        matrix.M33 = M33;
        return matrix;
    }
}