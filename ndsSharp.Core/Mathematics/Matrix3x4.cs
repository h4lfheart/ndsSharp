using System.Numerics;

namespace ndsSharp.Core.Mathematics;

public struct Matrix3x4(
    float m11, float m12, float m13, float m14,
    float m21, float m22, float m23, float m24,
    float m31, float m32, float m33, float m34)
{
    public static readonly Matrix3x4 Zero = new();
    public static readonly Matrix3x4 Identity = new()
    {
        M11 = 1.0f, M21 = 0.0f, M31 = 0.0f,
        M12 = 0.0f, M22 = 1.0f, M32 = 0.0f,
        M13 = 0.0f, M23 = 0.0f, M33 = 1.0f,
        M14 = 0.0f, M24 = 0.0f, M34 = 0.0f
    };

    public float M11 = m11; // Column 1
    public float M12 = m12;
    public float M13 = m13;
    public float M14 = m14;
    public float M21 = m21; // Column 2
    public float M22 = m22;
    public float M23 = m23;
    public float M24 = m24;
    public float M31 = m31; // Column 3
    public float M32 = m32;
    public float M33 = m33;
    public float M34 = m34;

    public Matrix4x4 To4x4()
    {
        var matrix = Matrix4x4.Identity;
        matrix.M11 = M11;
        matrix.M12 = M12;
        matrix.M13 = M13;
        matrix.M14 = M14;
        matrix.M21 = M21;
        matrix.M22 = M22;
        matrix.M23 = M23;
        matrix.M24 = M24;
        matrix.M31 = M31;
        matrix.M32 = M32;
        matrix.M33 = M33;
        matrix.M34 = M34;
        return matrix;
    }
}