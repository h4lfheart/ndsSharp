using System.Runtime.CompilerServices;

namespace ndsSharp.Core.Extensions;

public static class StringExtensions
{
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Flip(this string str)
    {
        var characters = str.ToCharArray();
        Array.Reverse(characters);
        return new string(characters);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string SubstringBefore(this string s, char delimiter)
    {
        var index = s.IndexOf(delimiter);
        return index == -1 ? s : s.Substring(0, index);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string SubstringBefore(this string s, string delimiter, StringComparison comparisonType = StringComparison.Ordinal)
    {
        var index = s.IndexOf(delimiter, comparisonType);
        return index == -1 ? s : s.Substring(0, index);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string SubstringAfter(this string s, char delimiter)
    {
        var index = s.IndexOf(delimiter);
        return index == -1 ? s : s.Substring(index + 1, s.Length - index - 1);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string SubstringAfter(this string s, string delimiter, StringComparison comparisonType = StringComparison.Ordinal)
    {
        var index = s.IndexOf(delimiter, comparisonType);
        return index == -1 ? s : s.Substring(index + delimiter.Length, s.Length - index - delimiter.Length);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string SubstringBeforeLast(this string s, char delimiter)
    {
        var index = s.LastIndexOf(delimiter);
        return index == -1 ? s : s.Substring(0, index);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string SubstringBeforeWithLast(this string s, char delimiter)
    {
        var index = s.LastIndexOf(delimiter);
        return index == -1 ? s : s.Substring(0, index + 1);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string SubstringBeforeLast(this string s, string delimiter, StringComparison comparisonType = StringComparison.Ordinal)
    {
        var index = s.LastIndexOf(delimiter, comparisonType);
        return index == -1 ? s : s.Substring(0, index);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string SubstringAfterLast(this string s, char delimiter)
    {
        var index = s.LastIndexOf(delimiter);
        return index == -1 ? s : s.Substring(index + 1, s.Length - index - 1);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string SubstringAfterWithLast(this string s, char delimiter)
    {
        var index = s.LastIndexOf(delimiter);
        return index == -1 ? s : s.Substring(index, s.Length - index);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string SubstringAfterLast(this string s, string delimiter, StringComparison comparisonType = StringComparison.Ordinal)
    {
        var index = s.LastIndexOf(delimiter, comparisonType);
        return index == -1 ? s : s.Substring(index + delimiter.Length, s.Length - index - delimiter.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains(this string orig, string value, StringComparison comparisonType) =>
        orig.IndexOf(value, comparisonType) >= 0;
}