namespace ndsSharp.Core.Extensions;

public static class MiscExtensions
{
    public static bool ChangeKey<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey oldKey, TKey newKey)
    {
        if (!dict.Remove(oldKey, out var value))
            return false;

        dict[newKey] = value;
        return true;
    }
}