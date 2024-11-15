using System.Numerics;

namespace ndsSharp.Core.Mathematics;

public struct Matrix3x3(
    float m11, float m12, float m13,
    float m21, float m22, float m23,
    float m31, float m32, float m33)
{
    public static readonly Matrix3x3 Zero = new();
    public static readonly Matrix3x3 Identity = new() { M11 = 1.0f, M22 = 1.0f, M33 = 1.0f };

    public float M11 = m11;
    public float M12 = m12;
    public float M13 = m13;
    public float M21 = m21;
    public float M22 = m22;
    public float M23 = m23;
    public float M31 = m31;
    public float M32 = m32;
    public float M33 = m33;

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