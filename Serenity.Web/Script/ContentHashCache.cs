using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;

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
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BufferedStream bs = new BufferedStream(fs))
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] hash = md5.ComputeHash(bs);
                return HttpServerUtility.UrlTokenEncode(hash);
            }
        }

        public static string ResolveWithHash(string contentUrl)
        {
            if (contentUrl.IsNullOrEmpty())
                throw new ArgumentNullException("contentUrl");

            contentUrl  = VirtualPathUtility.ToAbsolute(contentUrl);

            if (contentUrl.IndexOf(".axd/", StringComparison.OrdinalIgnoreCase) >= 0)
                return contentUrl;

            if (HttpContext.Current == null)
                throw new InvalidOperationException();

            var contentPath = HttpContext.Current.Server.MapPath(contentUrl);

            object hash;
            hash = hashByContentPath[contentPath];
            if (hash == null)
            {
                if (File.Exists(contentPath))
                    hash = GetFileSHA1(contentPath);
                else
                    hash = DateTime.Now.ToString("yyyymmddhh");

                Hashtable.Synchronized(hashByContentPath)[contentPath] = hash;
            }

            return VirtualPathUtility.ToAbsolute(contentUrl) + "?v=" + (string)hash;
        }
    }
}