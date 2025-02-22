namespace ndsSharp.Core.Conversion.Sounds.Decoding;

public static class ADPCM
{
    public static readonly int[] ImaIndexTable =
    [
        -1, -1, -1, -1, 2, 4, 6, 8,
        -1, -1, -1, -1, 2, 4, 6, 8
    ];

    public static readonly int[] ImaStepSizeTable =
    [
        7, 8, 9, 10, 11, 12, 13, 14, 16, 17,
        19, 21, 23, 25, 28, 31, 34, 37, 41, 45,
        50, 55, 60, 66, 73, 80, 88, 97, 107, 118,
        130, 143, 157, 173, 190, 209, 230, 253, 279, 307,
        337, 371, 408, 449, 494, 544, 598, 658, 724, 796,
        876, 963, 1060, 1166, 1282, 1411, 1552, 1707, 1878, 2066,
        2272, 2499, 2749, 3024, 3327, 3660, 4026, 4428, 4871, 5358,
        5894, 6484, 7132, 7845, 8630, 9493, 10442, 11487, 12635, 13899,
        15289, 16818, 18500, 20350, 22385, 24623, 27086, 29794, 32767
    ];

    public static byte[] Decode(byte[] waveData, int numSamples)
    {
        var result = new List<byte>();
        var decodedSamples = DecodeToSamples(waveData, numSamples);

        foreach (var sample in decodedSamples)
        {
            result.AddRange(BitConverter.GetBytes(sample));
        }

        return result.ToArray();
    }

    private static short[] DecodeToSamples(byte[] waveData, int numSamples)
    {
        var outBuff = new short[numSamples]; /* output buffer pointer */
        var outIndex = 0; /* Output buffer pointer */

        var state = new ADPCMState();
        var inp /* Input buffer pointer */ = 0;
        var inputbuffer /* Place to keep next 4-bit value */ = 0;

        var valpred /* Predicted value */ = state.Previous;
        var index /* Current step change index */ = state.Index;
        var step /* Stepsize */ = ImaStepSizeTable[index];

        var bufferstep /* Toggle between inputbuffer/input */ = false;

        for (; numSamples > 0; numSamples--)
        {
            /* Step 1 - get the delta value */
            int delta; /* Current adpcm output value */
            if (bufferstep)
            {
                delta = inputbuffer & 0xf;
            }
            else
            {
                inputbuffer = waveData[inp++];
                delta = (inputbuffer >> 4) & 0xf;
            }

            bufferstep = !bufferstep;

            /* Step 2 - Find new index value (for later) */
            index += ImaIndexTable[delta];
            if (index < 0) index = 0;
            if (index > 88) index = 88;

            /* Step 3 - Separate sign and magnitude */
            var sign = delta & 8; /* Current adpcm sign bit */
            delta &= 7;

            /* Step 4 - Compute difference and new predicted value */
            /*
             ** Computes 'vpdiff = (delta+0.5)*step/4', but see comment
             ** in adpcm_coder.
             */
            var vpdiff = step >> 3; /* Current change to valpred */
            if ((delta & 4) != 0) vpdiff += step;
            if ((delta & 2) != 0) vpdiff += step >> 1;
            if ((delta & 1) != 0) vpdiff += step >> 2;

            if (sign != 0)
                valpred -= vpdiff;
            else
                valpred += vpdiff;

            /* Step 5 - clamp output value */
            if (valpred > short.MaxValue)
                valpred = short.MaxValue;
            else if (valpred < short.MinValue)
                valpred = short.MinValue;

            /* Step 6 - Update step value */
            step = ImaStepSizeTable[index];

            /* Step 7 - Output value */
            outBuff[outIndex++] = (short)valpred;
        }

        state.Previous = valpred;
        state.Index = index;

        return outBuff;
    }
}

public class ADPCMState
{
    public int Index;
    public int Previous;
}