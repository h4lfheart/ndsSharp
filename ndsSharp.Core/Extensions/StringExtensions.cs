namespace ndsSharp.Core.Extensions;

public static class StringExtensions
{
    public static string Flip(this string str)
    {
        return new string(str.Reverse().ToArray());
    }
}