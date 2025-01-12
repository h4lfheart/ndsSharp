using System.Numerics;

namespace ndsSharp.Core.Extensions;

public static class BitExtensions
{
    public static byte Bits(this byte value, int lo, int hi)
    {
        return (byte) ((value >> lo) & ((1 << (hi - lo)) - 1));
    }

    public static ushort Bits(this ushort value, int lo, int hi)
    {
        return (ushort) ((value >> lo) & ((1 << (hi - lo)) - 1));
    }

    public static uint Bits(this uint value, int lo, int hi)
    {
        return (uint) ((value >> lo) & ((1 << (hi - lo)) - 1));
    }
    
    public static uint Bits(this int value, int lo, int hi)
    {
        return (uint) ((value >> lo) & ((1 << (hi - lo)) - 1));
    }
    
    public static byte Bit(this byte value, int index)
    {
        return Bits(value, index, index + 1);
    }

    public static ushort Bit(this ushort value, int index)
    {
        return Bits(value, index, index + 1);
    }

    public static uint Bit(this uint value, int index)
    {
        return Bits(value, index, index + 1);
    }
    
    public static uint Bit(this int value, int index)
    {
        return Bits(value, index, index + 1);
    }
}