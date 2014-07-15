using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public static partial class Q
    {
        /// <summary>
        /// Resolves a "~/..." URL to absolute path, using Q.Config.ApplicationPath
        /// </summary>
        /// <param name="url">URL to be resolved</param>
        /// <returns>Absolute URL with tilde removed</returns>
        public static string ResolveUrl(string url)
        {
            if (url != null && url.Length > 0 && url.Substr(0, 2) == "~/")
                return Q.Config.ApplicationPath + url.Substr(2);
            else
                return url;
        }

        public static void AutoOpenByQuery(string key, Action<string> autoOpen)
        {
            var query = Q.Externals.ParseQueryString();
            var value = query[key];
            if (value != null)
                autoOpen(value);
        }

        public static void AutoOpenByQueryID(string key, Action<Int64> autoOpen)
        {
            AutoOpenByQuery(key, delegate(string value)
            {
                var id = value.ConvertToId();
                if (id == null || Double.IsNaN(id.As<double>()))
                    return;

                autoOpen(id.Value);
            });
        }
    }
}