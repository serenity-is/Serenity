#if !ASPNETMVC
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Serenity;
using Microsoft.AspNetCore.Http;
using System;
#if !ASPNETCORE22
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IWebHostEnvironment;
#endif

namespace Serenity.Web
{
    public static class HttpRequestExtensions
    {
        public static Uri GetBaseUri(this HttpRequest request)
        {
            var hostComponents = request.Host.ToUriComponent().Split(':');

            var builder = new UriBuilder
            {
                Scheme = request.Scheme,
                Host = hostComponents[0]
            };

            if (hostComponents.Length == 2)
                builder.Port = Convert.ToInt32(hostComponents[1]);

            return builder.Uri;
        }
    }
}

namespace System.Web.Hosting
{
    public static class HostingEnvironment
    {
        public static string MapPath(string path)
        {
            if (path.IsEmptyOrNull())
                return path;

            path = path.Replace('\\', '/');

            if (path.IndexOf("..") >= 0 ||
                path.IndexOf("//") >= 0 ||
                path.IndexOf(":/") >= 0 ||
                path.IndexOfAny(new char[] { '*', '?', '>', '<', ',', ':', ';', '\'', '"', ']', '?' }) >= 0)
                throw new ArgumentOutOfRangeException("path");

            var webRootPath = Dependency.Resolve<IHostingEnvironment>().WebRootPath;

            if (path.StartsWith("~/"))
                path = path.Substring(2);
            else if (path.StartsWith("/"))
            {
                var applicationRoot = VirtualPathUtility.ToAbsolute("~/");
                if (path.StartsWith(applicationRoot, StringComparison.OrdinalIgnoreCase))
                    path = path.Substring(applicationRoot.Length);
                else
                    throw new ArgumentOutOfRangeException("path");
            }

            path = Path.Combine(webRootPath, path.Replace('/', Path.DirectorySeparatorChar));
            if (!path.StartsWith(webRootPath, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentOutOfRangeException("path");

            return path;
        }

        public static string ApplicationVirtualPath
        {
            get
            {
                return VirtualPathUtility.ToAbsolute("~/");
            }
        }
    }
}
#endif