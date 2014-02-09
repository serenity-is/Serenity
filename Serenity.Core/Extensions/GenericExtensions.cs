using System;
using System.Collections.Generic;
using System.Linq;

namespace Serenity
{
    public static class GenericExtensions
    {
        public static bool EqualsAnyOf<T>(this T source, params T[] list)
        {
            if (null == source) 
                throw new ArgumentNullException("source");

            return list.Contains(source);
        }

        public static string FormatWith(this string format, params object[] args)
        {
            return String.Format(format, args);
        }

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

        public static void IfNotNull<T>(this T obj, Action<T> action) where T: class
        {
            if (obj != null)
                action(obj);
        }

        public static T As<T>(this object item) where T : class
        {
            return item as T;
        }
    }
}