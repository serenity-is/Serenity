#if COREFX
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using Serenity;

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