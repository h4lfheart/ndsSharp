using ndsSharp.Core.Data;
using ndsSharp.Core.Extensions;

namespace ndsSharp.Core.Objects.Exports.Animation.Anim;

public class JNTCurve<TDataType>
{
    public JNTCurveInfo CurveInfo;
    public TDataType[] Samples = [];

    public static JNTCurve<TDataType> Empty => new();
    
    public JNTCurve()
    {
        CurveInfo = new JNTCurveInfo();
    }

    public JNTCurve(TDataType constantValue)
    {
        CurveInfo = new JNTCurveInfo();
        Samples = [constantValue];
    }
    
    public JNTCurve(JNTCurveInfo curveInfo, TDataType[] samples)
    {
        CurveInfo = curveInfo;
        Samples = samples;
    }
    
}

public class JNTCurveInfo : BaseDeserializable
{
    public uint StartFrame;
    public uint EndFrame;
    public uint DataWidth;
    public uint LogRate;

    public uint NumSamples => (EndFrame - StartFrame) / Rate;
    public uint Rate => (uint) Math.Pow(2, LogRate);

    public uint SampleOffset;
    
    public override void Deserialize(DataReader reader)
    {
        var flag = reader.Read<uint>();

        StartFrame = flag.Bits(0, 16);
        EndFrame = flag.Bits(16, 28);
        DataWidth = flag.Bits(28, 30);
        LogRate = flag.Bits(30, 32);

        SampleOffset = reader.Read<uint>();
    }
}