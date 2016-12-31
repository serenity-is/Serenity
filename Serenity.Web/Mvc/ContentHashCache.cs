using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Web.Hosting;
using System.Web;
#if ASPNETCORE
using Microsoft.AspNetCore.WebUtilities;
#endif

namespace Serenity.Web
{
    /// <summary>
    ///   Static class which contains javascript helper functions</summary>
    public static class ContentHashCache
    {
        private static Hashtable hashByContentPath;

        static ContentHashCache()
        {
            hashByContentPath = new Hashtable(StringComparer.OrdinalIgnoreCase);
        }

        public static void ScriptsChanged()
        {
            hashByContentPath = new Hashtable(StringComparer.OrdinalIgnoreCase);
        }

        private static string GetFileSHA1(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 120000))
            { 
#if ASPNETCORE
                var md5 = MD5.Create();
                byte[] hash = md5.ComputeHash(fs);
                return WebEncoders.Base64UrlEncode(hash);
#else
                using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
                {
                    byte[] hash = md5.ComputeHash(fs);
                    return HttpServerUtility.UrlTokenEncode(hash);
                }
#endif
            }
        }

        public static string ResolveWithHash(string contentUrl)
        {
            if (contentUrl.IsNullOrEmpty())
                throw new ArgumentNullException("contentUrl");

            if (contentUrl.IndexOf(".axd/", StringComparison.OrdinalIgnoreCase) >= 0)
                return contentUrl;

            var path = contentUrl;
            path = HostingEnvironment.MapPath(path);

            object hash;
            hash = hashByContentPath[path];
            if (hash == null)
            {
                if (File.Exists(path))
                    hash = GetFileSHA1(path);
                else
                    hash = DateTime.Now.ToString("yyyymmddhh");

                Hashtable.Synchronized(hashByContentPath)[path] = hash;
            }

            return VirtualPathUtility.ToAbsolute(contentUrl) + "?v=" + (string)hash;
        }
    }
}