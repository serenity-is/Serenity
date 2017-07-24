#if ASPNETCORE
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Serenity;
using Microsoft.AspNetCore.Http;
using System;

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

            if (path.IndexOf("..") >= 0 ||
                path.IndexOf("//") >= 0 ||
                path.IndexOf("\\\\") >= 0 ||
                path.IndexOfAny(new char[] { '*', '?', '>', '<', ',', ':', ';', '\'', '"', ']' }) >= 0 ||
                path.IndexOf('?') >= 0 ||
                path.IndexOf('>') >= 0)
                throw new ArgumentOutOfRangeException("path");

            if (path.StartsWith("~/"))
                path = path.Substring(2);
            else if (path.StartsWith("/"))
                path = path.Substring(1);

            return Path.Combine(Dependency.Resolve<IHostingEnvironment>().WebRootPath, path);
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