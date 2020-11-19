using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Serenity.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Web;
using System.Web.Hosting;

namespace Serenity.Web
{
    public class ContentHashCache
    {
        private Hashtable hashByContentPath;
        private readonly bool cdnEnabled;
        private readonly string cdnHttp;
        private readonly string cdnHttps;
        private readonly GlobFilter cdnFilter;
        private readonly IHttpContextAccessor httpContextAccessor;

        public class CDNSettings
        {
            public bool? Enabled { get; set; }
            public string Url { get; set; }
            public string HttpsUrl { get; set; }
            public List<string> Include { get; set; }
            public List<string> Exclude { get; set; }
        }

        public ContentHashCache(IOptions<CDNSettings> cdnSettings, IHttpContextAccessor httpContextAccessor = null)
        {
            this.httpContextAccessor = httpContextAccessor;

            var cdn = cdnSettings.Value;
            hashByContentPath = new Hashtable(StringComparer.OrdinalIgnoreCase);
            cdnEnabled = cdn.Enabled == true && !string.IsNullOrEmpty(cdn.Url);
            cdnHttp = cdn.Url;
            if (string.IsNullOrEmpty(cdn.HttpsUrl))
                cdnHttps = (cdnHttp ?? "").Replace("http://", "https://");
            else
                cdnHttps = cdn.HttpsUrl;

            cdnFilter = new GlobFilter(cdn.Include, cdn.Exclude);
        }

        public void ScriptsChanged()
        {
            hashByContentPath = new Hashtable(StringComparer.OrdinalIgnoreCase);
        }

        private static string GetFileSHA1(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 120000))
            { 
                var md5 = MD5.Create();
                byte[] hash = md5.ComputeHash(fs);
                return WebEncoders.Base64UrlEncode(hash);
            }
        }

        public string ResolvePath(string contentPath)
        {
            if (contentPath.IsNullOrEmpty())
                throw new ArgumentNullException("contentPath");

            if (contentPath[0] != '/' &&
                (contentPath[0] != '~' || contentPath.Length < 2 || contentPath[1] != '/'))
                return contentPath;

            contentPath = VirtualPathUtility.ToAbsolute(contentPath);
            if (!cdnEnabled)
                return contentPath;

            bool isSecureConnection = httpContextAccessor?.HttpContext?.Request?.IsHttps == true;

            string cdnRoot = isSecureConnection ? cdnHttps : cdnHttp;
            return UriHelper.Combine(cdnRoot, contentPath);
        }

        public string ResolveWithHash(string contentUrl)
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

            bool isSecureConnection = httpContextAccessor?.HttpContext?.Request?.IsHttps == true;

            string cdnRoot = isSecureConnection ? cdnHttps : cdnHttp;
            contentUrl = VirtualPathUtility.ToAbsolute(contentUrl);
            return UriHelper.Combine(cdnRoot, contentUrl);
        }
    }
}