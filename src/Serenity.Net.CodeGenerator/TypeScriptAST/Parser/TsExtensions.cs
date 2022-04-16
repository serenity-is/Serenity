using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Sdcb.TypeScript.TsParser
{
    public static class TsExtensions
    {
        public static int charCodeAt(this string str, int pos) => (int)str[pos];
        //public static int length(this string str) => str.Length;
        public static string substring(this string str, int start, int? end = null)
        {
            return end == null ? str.Substring(start) : str.Substring(start, (int)end - start);
        }
        public static string[] exec(this Regex r, string text) => r.Match(text).Captures.Cast<string>().ToArray();
        public static bool test(this Regex r, string text) => r.IsMatch(text);
        public static void pop<T>(this List<T> list) => list.RemoveAt(0);

        
    }
    public static class String
    {
        internal static string fromCharCode(params int[] codes)
        {
            var sb = new StringBuilder();
            foreach (var c in codes)
            {
                sb.Append((char)c);
            }
            return sb.ToString();
        }
        public static string slice(this string str, int start, int end = int.MaxValue)
        {
            if (start < 0)
                start += str.Length;
            if (end < 0)
                end += str.Length;

            start = Math.Min(Math.Max(start, 0), str.Length);
            end = Math.Min(Math.Max(end, 0), str.Length);
            if (end <= start)
                return string.Empty;

            return str.Substring(start, end - start);
        }

    }
}
