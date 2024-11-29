using System.Diagnostics;
using System.Numerics;
using ndsSharp.Core.Data;
using ndsSharp.Core.Extensions;
using ndsSharp.Core.Mathematics;
using ndsSharp.Core.Objects.Exports.Meshes.Model;

namespace ndsSharp.Core.Objects.Exports.Animation.Anim;

public class JNTTrack : BaseDeserializable
{
    public JNTAnimation Owner;
    
    public JNTTrackFlags Flags;

    public JNTCurve<float>[] TranslationCurves = [JNTCurve<float>.Empty, JNTCurve<float>.Empty, JNTCurve<float>.Empty];
    public JNTCurve<Matrix3x3> RotationCurve = JNTCurve<Matrix3x3>.Empty;
    public JNTCurve<float>[] ScaleCurves = [JNTCurve<float>.Empty, JNTCurve<float>.Empty, JNTCurve<float>.Empty];
    
    public byte Index;

    private const int PIVOT_MATRIX_SIZE = sizeof(ushort) * 3;
    private const int BASIS_MATRIX_SIZE = sizeof(ushort) * 5;
    
    public override void Deserialize(BaseReader reader)
    {
        Flags = reader.ReadObject<JNTTrackFlags>();
        reader.Position += sizeof(byte);
        Index = reader.Read<byte>();
        
        if (!Flags.IsAnimated) return;

        if (Flags.HasTranslation)
        {
            for (var channelIndex = 0; channelIndex < 3; channelIndex++)
            {
                TranslationCurves[channelIndex] = ReadTranslationCurve(reader, Flags.ConstantTranslationFlags[channelIndex]);
            }
        }

        if (Flags.HasRotation)
        {
            RotationCurve = ReadRotationCurve(reader, Flags.ConstantRotationFlag);
        }
        
        if (Flags.HasScale)
        {
            for (var channelIndex = 0; channelIndex < 3; channelIndex++)
            {
                ScaleCurves[channelIndex] = ReadScaleCurve(reader, Flags.ConstantScaleFlags[channelIndex]);
            }
        }
        
    }

    public JNTCurve<float> ReadTranslationCurve(BaseReader reader, bool isConstant)
    {
        if (isConstant)
        {
            var constantValue = reader.ReadIntAsFloat();
            return new JNTCurve<float>(constantValue);
        }

        var curveInfo = reader.ReadObject<JNTCurveInfo>();
        
        var samples = reader.Peek(() =>
        {
            reader.Position = (int)curveInfo.SampleOffset;
            return reader.ReadArray((int) curveInfo.NumSamples, () => curveInfo.DataWidth == 0 ? reader.ReadIntAsFloat() : reader.ReadShortAsFloat());
        });

        return new JNTCurve<float>(curveInfo, samples);
    }
    
    public JNTCurve<Matrix3x3> ReadRotationCurve(BaseReader reader, bool isConstant)
    {
        if (isConstant)
        {
            var controlData = reader.Read<ushort>();
            reader.Position += sizeof(ushort); // no clue don't ask

            var matrix = reader.Peek(() => ReadMatrix(reader, controlData));
            return new JNTCurve<Matrix3x3>(matrix);
        }

        var curveInfo = reader.ReadObject<JNTCurveInfo>();
        var samples = reader.Peek(() =>
        {
            reader.Position = (int) curveInfo.SampleOffset;
            return reader.ReadArray((int) curveInfo.NumSamples, () =>
            {
                var controlData = reader.Read<ushort>();
                return reader.Peek(() => ReadMatrix(reader, controlData));
            });
        });

        return new JNTCurve<Matrix3x3>(curveInfo, samples);
    }
    
    public JNTCurve<float> ReadScaleCurve(BaseReader reader, bool isConstant)
    {
        if (isConstant)
        {
            var constantValue = reader.ReadIntAsFloat();
            reader.Position += sizeof(uint);
            return new JNTCurve<float>(constantValue);
        }

        var curveInfo = reader.ReadObject<JNTCurveInfo>();
        
        var samples = reader.Peek(() =>
        {
            reader.Position = (int)curveInfo.SampleOffset;
            return reader.ReadArray((int) curveInfo.NumSamples, () =>
            {
                var sampleValue = curveInfo.DataWidth == 0 ? reader.ReadIntAsFloat() : reader.ReadShortAsFloat();
                reader.Position += curveInfo.DataWidth == 0 ? sizeof(uint) : sizeof(ushort);
                return sampleValue;
            });
        });

        return new JNTCurve<float>(curveInfo, samples);
    }

    private Matrix3x3 ReadMatrix(BaseReader reader, ushort controlData)
    {
        var matrixIndex = controlData.Bits(0, 15);
        
        var usePivotMatrix = controlData.Bit(15) == 1;
        if (usePivotMatrix)
        {
            reader.Position = (int) (Owner.PivotMatricesOffset + PIVOT_MATRIX_SIZE * matrixIndex);
            
            var flag = reader.Read<ushort>();
            var matrixSelection = flag.Bits(0, 4);
            var negateFlag = flag.Bits(4, 8);
            var a = reader.ReadShortAsFloat();
            var b = reader.ReadShortAsFloat();
            
            return Matrix3x3.CreatePivot(matrixSelection, negateFlag, a, b);
        }
        else
        {
            reader.Position = (int) (Owner.BasisMatricesOffset + BASIS_MATRIX_SIZE * matrixIndex);
            
            var data = reader.ReadArray<ushort>(5);
            return Matrix3x3.CreateBasis(data);
        }
    }
}

public class JNTTrackFlags : BaseDeserializable
{
    public bool IsAnimated;
    
    public bool HasTranslation;
    public readonly bool[] ConstantTranslationFlags = new bool[3];
    
    public bool HasRotation;
    public bool ConstantRotationFlag;
    
    public bool HasScale;
    public readonly bool[] ConstantScaleFlags = new bool[3];
    
    
    public override void Deserialize(BaseReader reader)
    {
        var flag = reader.Read<ushort>();

        IsAnimated = flag.Bit(0) == 0;
        HasTranslation = flag.Bits(1, 3) == 0;
        ConstantTranslationFlags[0] = flag.Bit(3) == 1;
        ConstantTranslationFlags[1] = flag.Bit(4) == 1;
        ConstantTranslationFlags[2] = flag.Bit(5) == 1;

        HasRotation = flag.Bits(6, 8) == 0;
        ConstantRotationFlag = flag.Bit(8) == 1;
        
        HasScale =  flag.Bits(9, 11) == 0;
        ConstantScaleFlags[0] = flag.Bit(11) == 1;
        ConstantScaleFlags[1] = flag.Bit(12) == 1;
        ConstantScaleFlags[2] = flag.Bit(13) == 1;
    }
}