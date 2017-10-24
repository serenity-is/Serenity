using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Web.Hosting;
using System.Web;
using Serenity.ComponentModel;
using System.Collections.Generic;
using Serenity.IO;
using System.Collections.Concurrent;
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
        private static bool cdnEnabled;
        private static string cdnHttp;
        private static string cdnHttps;
        private static GlobFilter cdnFilter;

        [SettingKey("CDNSettings"), SettingScope("Application")]
        private class CDNSettings
        {
            public bool? Enabled { get; set; }
            public string Url { get; set; }
            public string HttpsUrl { get; set; }
            public List<string> Include { get; set; }
            public List<string> Exclude { get; set; }
        }

        static ContentHashCache()
        {
            hashByContentPath = new Hashtable(StringComparer.OrdinalIgnoreCase);
            var cdn = Config.Get<CDNSettings>();
            cdnEnabled = cdn.Enabled == true && !string.IsNullOrEmpty(cdn.Url);
            cdnHttp = cdn.Url;
            if (string.IsNullOrEmpty(cdn.HttpsUrl))
                cdnHttps = (cdnHttp ?? "").Replace("http://", "https://");
            else
                cdnHttps = cdn.HttpsUrl;

            cdnFilter = new GlobFilter(cdn.Include, cdn.Exclude);
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

            if (contentUrl[0] != '/' &&
                (contentUrl[0] != '~' || contentUrl.Length < 2 || contentUrl[1] != '/'))
                return contentUrl;

            string cdnMatch = contentUrl;

            if (contentUrl.IndexOf(".axd/", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                if (!cdnEnabled)
                    return contentUrl;

                cdnMatch = contentUrl.Split('?')[0];
            }
            else
            {
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

                contentUrl = VirtualPathUtility.ToAbsolute(contentUrl) + "?v=" + (string)hash;
                if (!cdnEnabled)
                    return contentUrl;
            }

            if (cdnMatch[0] == '/')
            {
                var root = VirtualPathUtility.ToAbsolute("~/");
                if (cdnMatch.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                    cdnMatch = cdnMatch.Substring(root.Length);
            }
            else
                cdnMatch = cdnMatch.Substring(2);

            if (!cdnFilter.IsMatch(cdnMatch.Replace('/', Path.DirectorySeparatorChar)))
                return contentUrl;

#if ASPNETCORE
            var contextAccessor = Dependency.TryResolve<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            bool isSecureConnection = contextAccessor != null && contextAccessor.HttpContext != null &&
                contextAccessor.HttpContext.Request.IsHttps;
#else
            bool isSecureConnection = HttpContext.Current != null && 
                HttpContext.Current.Request.IsSecureConnection;
#endif
            string cdnRoot = isSecureConnection ? cdnHttps : cdnHttp;
            contentUrl = VirtualPathUtility.ToAbsolute(contentUrl);
            return UriHelper.Combine(cdnRoot, contentUrl);
        }
    }
}