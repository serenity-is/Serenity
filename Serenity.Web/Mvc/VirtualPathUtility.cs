#if COREFX
using Microsoft.AspNetCore.Http;
using System;

namespace Serenity.Web
{
    public static class VirtualPathUtility
    {
        public static string ToAbsolute(string contentPath)
        {
            var context = Dependency.Resolve<IHttpContextAccessor>().HttpContext;
            return GenerateClientUrl(context.Request.PathBase, contentPath);
        }

        private static string GenerateClientUrl(PathString applicationPath, string path)
        {
            if (path.StartsWith("~/", StringComparison.Ordinal))
            {
                var segment = new PathString(path.Substring(1));
                return applicationPath.Add(segment).Value;
            }

            return path;
        }
    }
}
#endif