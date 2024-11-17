using System.Runtime.InteropServices;

namespace ndsSharp.Core.Conversion.Sounds.Formats;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class Wave
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private char[] HeaderID = ['R', 'I', 'F', 'F'];

    [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
    private uint ChunkSize;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private char[] Format = ['W', 'A', 'V', 'E'];

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private char[] FormatHeaderID = ['f', 'm', 't', ' '];

    [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
    private uint FormatHeaderSize = 16;

    [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
    private WaveFormat AudioFormat = WaveFormat.WAVE_FORMAT_PCM;

    [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
    private ushort NumChannels;

    [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
    private uint SampleRate;

    [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
    private uint ByteRate;

    [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
    private ushort BlockAlign;

    [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
    private ushort BitsPerSample;
            
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private char[] DataHeaderID = ['d', 'a', 't', 'a'];
    
    [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
    private uint DataHeaderSize;

    public byte[] Data;

    public Wave(byte[] data, ushort numChannels, uint sampleRate, ushort bitsPerSample)
    {
        Data = data;
        NumChannels = numChannels;
        SampleRate = sampleRate;
        BitsPerSample = bitsPerSample;

        ChunkSize = (uint) (0x24 + data.Length);
        ByteRate = SampleRate * NumChannels * BitsPerSample / 8;
        BlockAlign = (ushort) (NumChannels * BitsPerSample / 8);
        DataHeaderSize = (uint) data.Length;
    }

    public byte[] GetBuffer()
    {
        var headerSize = Marshal.SizeOf(this);
        var headerPtr = Marshal.AllocHGlobal(headerSize);
        Marshal.StructureToPtr(this, headerPtr, false);
        var rawData = new byte[headerSize + Data.Length];
        Marshal.Copy(headerPtr, rawData, 0, headerSize);
        Marshal.FreeHGlobal(headerPtr);
        Array.Copy(Data, 0, rawData, 44, Data.Length);
        return rawData;
    }
}

public enum WaveFormat : ushort
{
    WAVE_FORMAT_PCM = 0x0001,
    IBM_FORMAT_ADPCM = 0x0002,
    IBM_FORMAT_MULAW = 0x0007,
    IBM_FORMAT_ALAW = 0x0006,
    WAVE_FORMAT_EXTENSIBLE = 0xFFFE
}