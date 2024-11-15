namespace ndsSharp.Core.Extensions;

public static class FloatExtensions
{
    public static float ToFloat(this int value, int signBits, int intBits, int fracBits)
    {
        double result;

        if (signBits == 0)
        {
            result = value;
        }
        else
        {
            var signMask = 1 << (intBits + fracBits);
            if ((value & signMask) != 0)
            {
                result = value | ~(signMask - 1);
            }
            else
            {
                result = value;
            }
        }

        return (float)(result / (1 << fracBits));
    }
}