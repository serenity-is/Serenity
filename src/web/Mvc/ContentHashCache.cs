using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Serenity.IO;
using System.Security.Cryptography;

namespace Serenity.Web;

/// <summary>
/// Default <see cref="IContentHashCache"/> implementation.
/// </summary>
public class ContentHashCache : IContentHashCache, IDisposable
{
    private readonly bool cdnEnabled;
    private readonly string? cdnHttp;
    private readonly string? cdnHttps;
    private readonly GlobFilter cdnFilter;
    private readonly IWebHostEnvironment hostEnvironment;
    private readonly IHttpContextAccessor? httpContextAccessor;
    private readonly IMemoryCache memoryCache;
    private readonly IWebRootFileWatcher? fileWatcher;
    private readonly IDisposable? ownedMemoryCache;
    private CancellationTokenSource tokenSource = new();

    /// <summary>
    /// CDN settings for the content hash cache.
    /// </summary>
    public class CDNSettings
    {
        /// <summary>
        /// Gets or sets whether the CDN is enabled.
        /// </summary>
        public bool? Enabled { get; set; }

        /// <summary>
        /// Gets or sets the CDN URL.
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// Gets or sets the HTTPS URL for the CDN.
        /// </summary>
        public string? HttpsUrl { get; set; }

        /// <summary>
        /// Gets or sets the list of include patterns.
        /// </summary>
        public List<string> Include { get; set; } = [];

        /// <summary>
        /// Gets or sets the list of exclude patterns.
        /// </summary>
        public List<string> Exclude { get; set; } = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentHashCache"/> class.
    /// </summary>
    /// <param name="cdnSettings">The CDN settings.</param>
    /// <param name="memoryCache">The memory cache.</param>
    /// <param name="hostEnvironment">The host environment.</param>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    /// <param name="fileWatcher">The web root file watcher, used to invalidate hashes in development.</param>
    /// <exception cref="ArgumentNullException"><paramref name="hostEnvironment"/> is <c>null</c>.</exception>
    public ContentHashCache(IOptions<CDNSettings> cdnSettings,
        IMemoryCache memoryCache,
        IWebHostEnvironment hostEnvironment,
        IHttpContextAccessor? httpContextAccessor = null,
        IWebRootFileWatcher? fileWatcher = null)
    {
        this.memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        this.hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        this.httpContextAccessor = httpContextAccessor;
        this.fileWatcher = fileWatcher;

        var cdn = cdnSettings.Value;
        cdnEnabled = cdn.Enabled == true && !string.IsNullOrEmpty(cdn.Url);
        cdnHttp = cdn.Url;
        if (string.IsNullOrEmpty(cdn.HttpsUrl))
            cdnHttps = (cdnHttp ?? "").Replace("http://", "https://", StringComparison.Ordinal);
        else
            cdnHttps = cdn.HttpsUrl;

        cdnFilter = new GlobFilter(cdn.Include, cdn.Exclude);
    }

    /// <summary>
    /// Initializes a new instance with a private memory cache.
    /// </summary>
    /// <param name="cdnSettings">The CDN settings.</param>
    /// <param name="hostEnvironment">The host environment.</param>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    /// <param name="fileWatcher">The web root file watcher, used to invalidate hashes in development.</param>
    public ContentHashCache(IOptions<CDNSettings> cdnSettings,
        IWebHostEnvironment hostEnvironment, IHttpContextAccessor? httpContextAccessor = null,
        IWebRootFileWatcher? fileWatcher = null)
        : this(cdnSettings, new MemoryCache(new MemoryCacheOptions()), hostEnvironment,
            httpContextAccessor, fileWatcher)
    {
        ownedMemoryCache = memoryCache;
    }

    /// <inheritdoc/>
    public void ScriptsChanged()
    {
        tokenSource.Cancel();
        lock (@lock)
            tokenSource = new CancellationTokenSource();
    }

    private readonly Lock @lock = new();

    private string GetFileSHA1(string webRelativePath, IFileInfo file, out IChangeToken? changeToken)
    {
        byte[] hash;
        changeToken = null;
        var md5 = MD5.Create();
        try
        {
            var physicalPath = file.PhysicalPath;
            if (physicalPath != null && System.IO.File.Exists(physicalPath))
            {
                changeToken = fileWatcher?.GetChangeToken(webRelativePath);

                try
                {
                    using System.IO.FileStream fs = new(physicalPath,
                        System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite, 120000);
                    hash = md5.ComputeHash(fs);
                }
                catch
                {
                    hash = md5.ComputeHash(Encoding.UTF8.GetBytes(System.IO.File.GetLastWriteTime(physicalPath).ToString("s")));
                }
            }
            else
            {
                try
                {
                    using var stream = file.CreateReadStream();
                    hash = md5.ComputeHash(stream);
                }
                catch
                {
                    hash = md5.ComputeHash(Encoding.UTF8.GetBytes(file.LastModified.ToString("s")));
                }
            }
        }
        catch
        {
            hash = md5.ComputeHash(Encoding.UTF8.GetBytes(TemporaryFileHelper.RandomFileCode()));
        }
        return WebEncoders.Base64UrlEncode(hash);
    }

    /// <inheritdoc/>
    public string ResolvePath(PathString pathBase, string contentPath)
    {
        if (string.IsNullOrEmpty(contentPath))
            throw new ArgumentNullException(nameof(contentPath));

        if (contentPath[0] != '/' &&
            (contentPath[0] != '~' || contentPath.Length < 2 || contentPath[1] != '/'))
            return contentPath;

        contentPath = VirtualPathUtility.ToAbsolute(pathBase, contentPath);
        if (!cdnEnabled)
            return contentPath;

        bool isSecureConnection = httpContextAccessor?.HttpContext?.Request?.IsHttps == true;

        string cdnRoot = (isSecureConnection ? cdnHttps : cdnHttp) ?? "";
        return UriHelper.Combine(cdnRoot, contentPath);
    }

    /// <inheritdoc/>
    public string ResolveWithHash(PathString pathBase, string contentUrl)
    {
        if (string.IsNullOrEmpty(contentUrl))
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
            var path = PathHelper.ToUrl(contentUrl[2..]);

            string cacheKey = "ContentHashCache_HashByContentPath_" + path;
            string hash;
            if (memoryCache.Get(cacheKey) is string cachedHash)
                hash = cachedHash;
            else
            {
                CancellationToken scriptsChangedToken;
                lock (@lock)
                    scriptsChangedToken = tokenSource.Token;

                var fileInfo = hostEnvironment.WebRootFileProvider.GetFileInfo(path);
                if (fileInfo.Exists)
                {
                    hash = GetFileSHA1(path, fileInfo, out var changeToken);
                    var cacheOpt = new MemoryCacheEntryOptions();
                    cacheOpt.AddExpirationToken(new CancellationChangeToken(scriptsChangedToken));
                    if (changeToken != null)
                        cacheOpt.AddExpirationToken(changeToken);

                    memoryCache.Set(cacheKey, hash, cacheOpt);
                }
                else
                    hash = DateTime.Now.ToString("yyyyMMddhh", CultureInfo.InvariantCulture);
            }

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

        string cdnRoot = (isSecureConnection ? cdnHttps : cdnHttp) ?? "";
        contentUrl = VirtualPathUtility.ToAbsolute(pathBase, contentUrl);
        return UriHelper.Combine(cdnRoot, contentUrl);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        ownedMemoryCache?.Dispose();
        GC.SuppressFinalize(this);
    }
}