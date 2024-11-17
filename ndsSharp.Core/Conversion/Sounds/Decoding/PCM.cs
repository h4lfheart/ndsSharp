namespace ndsSharp.Core.Conversion.Sounds.Decoding;

public static class PCM
{
    public static byte[] PCM8ToPCM16(byte[] byteData)
    {
        var result = new List<byte>();

        var shortData = ByteDataToShortData(byteData);
        for (var i = 0; i < shortData.Length; i += 2)
        {
            var sample = BitConverter.ToInt16(shortData, i);
            var pcm16 = (short)(sample & 0x7F);
            pcm16 <<= 8;
            if (sample >> 7 != 0)
                pcm16 -= 0x7FFF;

            result.AddRange(BitConverter.GetBytes(pcm16));
        }

        return result.ToArray();
    }
    
    public static byte[] ByteDataToShortData(byte[] data)
    {
        var result = new List<byte>();

        foreach (var bite in data)
        {
            result.AddRange(BitConverter.GetBytes((short) bite));
        }

        return result.ToArray();
    }
}