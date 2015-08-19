using System;
using System.Collections.Generic;
using System.Linq;

namespace Serenity
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> list, params T[] values)
        {
            foreach (T value in values)
                list.Add(value);
        }

        public static void AddRange<T>(this ICollection<T> list, IEnumerable<T> values)
        {
            foreach (T value in values)
                list.Add(value);
        }

        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            TValue value;
            if (!dict.TryGetValue(key, out value))
                return default(TValue);

            return value;
        }

        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue)
        {
            TValue value;
            if (!dict.TryGetValue(key, out value))
                return defaultValue;

            return value;
        }
    }
}