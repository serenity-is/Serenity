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
        [InlineCode("Q.resolveUrl({url})")]
        public static string ResolveUrl(string url)
        {
            if (url != null && url.Length > 0 && url.Substr(0, 2) == "~/")
                return Q.Config.ApplicationPath + url.Substr(2);
            else
                return url;
        }
    }
}