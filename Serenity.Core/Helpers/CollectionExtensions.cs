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
    }
}