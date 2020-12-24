using System;
using System.Collections.Generic;
using System.Linq;

namespace Serenity
{
    public static class PrefixHelper
    {
        public static int DeterminePrefixLength<T>(IEnumerable<T> list, Func<T, string> getName)
        {
            if (!Enumerable.Any(list))
                return 0;
            string str1 = getName(Enumerable.First(list));
            int length = str1.IndexOf('_');
            if (length <= 0)
                return 0;
            string str2 = str1.Substring(0, length);
            foreach (T obj in list)
            {
                if (!getName(obj).StartsWith(str2))
                    return 0;
            }
            return str2.Length + 1;
        }
    }
}