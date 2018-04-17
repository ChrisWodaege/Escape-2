using System.Collections.Generic;

public static class DictionaryExtensions
{
    public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> instance, TKey key, TValue value)
    {
        TValue outValue;
        if (instance.TryGetValue(key, out outValue))
        {
            return outValue;
        }

        instance.Add(key, value);
        return value;
    }

    public static void Update<TKey, TValue>(this Dictionary<TKey, TValue> instance, TKey key, TValue value)
    {
        instance.Remove(key);
        instance.Add(key, value);
    }
}
