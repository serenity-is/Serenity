using Microsoft.AspNetCore.Http;
using System;

namespace Serenity.Web
{
    public static class VirtualPathUtility
    {
        public static string ToAbsolute(IHttpContextAccessor accessor, string contentPath)
        {
            return ToAbsolute(accessor?.HttpContext, contentPath);
        }

        public static string ToAbsolute(HttpContext context, string contentPath)
        {
            return ToAbsolute(context?.Request?.PathBase ?? PathString.Empty, contentPath);
        }

        public static string ToAbsolute(PathString pathBase, string path)
        {
            if (!pathBase.HasValue)
                pathBase = PathString.Empty;

            if (path.StartsWith("~/", StringComparison.Ordinal))
            {
                var segment = new PathString(path[1..]);
                return pathBase.Add(segment).Value;
            }

            return path;
        }
    }
}