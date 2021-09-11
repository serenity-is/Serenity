using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;

namespace Serenity.Web
{
    public class ContentHashCache : IContentHashCache
    {
        private readonly ConcurrentDictionary<string, string> hashByContentPath;
        private readonly bool cdnEnabled;
        private readonly string cdnHttp;
        private readonly string cdnHttps;
        private readonly IO.GlobFilter cdnFilter;
        private readonly IWebHostEnvironment hostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;

        public class CDNSettings
        {
            public bool? Enabled { get; set; }
            public string Url { get; set; }
            public string HttpsUrl { get; set; }
            public List<string> Include { get; set; }
            public List<string> Exclude { get; set; }
        }

        public ContentHashCache(IOptions<CDNSettings> cdnSettings,
            IWebHostEnvironment hostEnvironment, IHttpContextAccessor httpContextAccessor = null)
        {
            this.hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
            this.httpContextAccessor = httpContextAccessor;

            var cdn = cdnSettings.Value;
            hashByContentPath = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            cdnEnabled = cdn.Enabled == true && !string.IsNullOrEmpty(cdn.Url);
            cdnHttp = cdn.Url;
            if (string.IsNullOrEmpty(cdn.HttpsUrl))
                cdnHttps = (cdnHttp ?? "").Replace("http://", "https://", StringComparison.Ordinal);
            else
                cdnHttps = cdn.HttpsUrl;

            cdnFilter = new IO.GlobFilter(cdn.Include, cdn.Exclude);
        }

        public void ScriptsChanged()
        {
            hashByContentPath.Clear();
        }

        private static string GetFileSHA1(IFileInfo file)
        {
            byte[] hash;
            var md5 = MD5.Create();
            var physicalPath = file.PhysicalPath;
            if (physicalPath != null)
            {
                using System.IO.FileStream fs = new(physicalPath,
                    System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read, 120000);
                hash = md5.ComputeHash(fs);
            }
            else
            {
                using var stream = file.CreateReadStream();
                hash = md5.ComputeHash(stream);
            }
            return WebEncoders.Base64UrlEncode(hash);
        }

        public string ResolvePath(PathString pathBase, string contentPath)
        {
            if (contentPath.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(contentPath));

            if (contentPath[0] != '/' &&
                (contentPath[0] != '~' || contentPath.Length < 2 || contentPath[1] != '/'))
                return contentPath;

            contentPath = VirtualPathUtility.ToAbsolute(pathBase, contentPath);
            if (!cdnEnabled)
                return contentPath;

            bool isSecureConnection = httpContextAccessor?.HttpContext?.Request?.IsHttps == true;

            string cdnRoot = isSecureConnection ? cdnHttps : cdnHttp;
            return UriHelper.Combine(cdnRoot, contentPath);
        }

        public string ResolveWithHash(PathString pathBase, string contentUrl)
        {
            if (contentUrl.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(contentUrl));

            if (contentUrl[0] == '~')
            {
                if (contentUrl.Length < 2 || contentUrl[1] != '/')
                    return VirtualPathUtility.ToAbsolute(pathBase, contentUrl);
            }
            else if (contentUrl[0] == '/')
            {
                if (contentUrl.Length < 2)
                    return contentUrl;

                var v = pathBase.Value ?? "";
                if (v.Length == 0 || v[^1] != '/')
                    v += '/';
                if (!contentUrl.StartsWith(v, StringComparison.OrdinalIgnoreCase))
                    return contentUrl;
                contentUrl = "~/" + contentUrl[v.Length..];
            }

            string cdnMatch = contentUrl;

            if (contentUrl.Contains(".axd/", StringComparison.OrdinalIgnoreCase))
            {
                if (!cdnEnabled)
                    return VirtualPathUtility.ToAbsolute(pathBase, contentUrl);

                cdnMatch = contentUrl.Split('?')[0];
            }
            else
            {
                var path = PathHelper.ToPath(contentUrl[2..]);

                var hash = hashByContentPath.GetOrAdd(path, (filePath) => 
                {
                    var fileInfo = hostEnvironment.WebRootFileProvider.GetFileInfo(filePath);
                    if (fileInfo.Exists)
                        return GetFileSHA1(fileInfo);
                    else
                        return DateTime.Now.ToString("yyyyMMddhh", CultureInfo.InvariantCulture);
                });

                contentUrl = VirtualPathUtility.ToAbsolute(pathBase, contentUrl) + "?v=" + hash;
                if (!cdnEnabled)
                    return contentUrl;
            }

            if (cdnMatch[0] == '/')
            {
                var root = VirtualPathUtility.ToAbsolute(pathBase, "~/");
                if (cdnMatch.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                    cdnMatch = cdnMatch[root.Length..];
            }
            else
                cdnMatch = cdnMatch[2..];

            if (!cdnFilter.IsMatch(cdnMatch.Replace('/', System.IO.Path.DirectorySeparatorChar)))
                return contentUrl;

            bool isSecureConnection = httpContextAccessor?.HttpContext?.Request?.IsHttps == true;

            string cdnRoot = isSecureConnection ? cdnHttps : cdnHttp;
            contentUrl = VirtualPathUtility.ToAbsolute(pathBase, contentUrl);
            return UriHelper.Combine(cdnRoot, contentUrl);
        }
    }
}