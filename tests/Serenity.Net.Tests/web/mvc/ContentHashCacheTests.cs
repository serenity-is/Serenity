using Microsoft.Extensions.FileProviders;

namespace Serenity.Web;

public class ContentHashCacheTests
{
    private sealed class TestFileInfo : IFileInfo
    {
        public bool Exists { get; set; } = true;
        public long Length => 5;
        public string? PhysicalPath { get; set; }
        public string Name => "site.js";
        public DateTimeOffset LastModified { get; set; } = DateTimeOffset.UtcNow;
        public bool IsDirectory => false;
        public Func<System.IO.Stream>? CreateReadStreamCallback { get; set; }

        public System.IO.Stream CreateReadStream() => CreateReadStreamCallback?.Invoke() ??
            new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes("content"));
    }

    private sealed class TestFileProvider(IFileInfo info) : IFileProvider
    {
        private readonly IFileInfo info = info;
        public IDirectoryContents GetDirectoryContents(string subpath) => throw new NotImplementedException();
        public IFileInfo GetFileInfo(string subpath) => info;
        public IChangeToken Watch(string filter) => throw new NotImplementedException();
    }

    private static ContentHashCache CreateWithFileInfo(IFileInfo info,
        ContentHashCache.CDNSettings? cdn = null, IHttpContextAccessor? accessor = null)
    {
        var env = new MockHostEnvironment
        {
            WebRootFileProvider = new TestFileProvider(info)
        };
        var options = Microsoft.Extensions.Options.Options.Create(cdn ?? new ContentHashCache.CDNSettings());
        return new ContentHashCache(options, env, accessor);
    }

    [Fact]
    public void ResolveWithHash_Hashes_Physical_File()
    {
        var file = System.IO.Path.GetTempFileName();
        System.IO.File.WriteAllText(file, "content");
        try
        {
            var result = CreateWithFileInfo(new TestFileInfo { PhysicalPath = file })
                .ResolveWithHash(PathString.Empty, "~/Scripts/site.js");

            Assert.StartsWith("/Scripts/site.js?v=", result);
        }
        finally
        {
            System.IO.File.Delete(file);
        }
    }

    [Fact]
    public void ResolveWithHash_Falls_Back_To_LastWrite_When_Physical_Read_Fails()
    {
        var result = CreateWithFileInfo(new TestFileInfo { PhysicalPath = System.IO.Path.GetTempPath() })
            .ResolveWithHash(PathString.Empty, "~/Scripts/site.js");

        Assert.StartsWith("/Scripts/site.js?v=", result);
    }

    [Fact]
    public void ResolveWithHash_Falls_Back_To_LastModified_When_Stream_Fails()
    {
        var result = CreateWithFileInfo(new TestFileInfo
        {
            CreateReadStreamCallback = () => throw new System.IO.IOException("fail")
        }).ResolveWithHash(PathString.Empty, "~/Scripts/site.js");

        Assert.StartsWith("/Scripts/site.js?v=", result);
    }

    [Fact]
    public void ResolveWithHash_Uses_Random_Code_When_All_Fail()
    {
        var result = CreateWithFileInfo(new TestFileInfo { PhysicalPath = "\0invalid" })
            .ResolveWithHash(PathString.Empty, "~/Scripts/site.js");

        Assert.StartsWith("/Scripts/site.js?v=", result);
    }

    [Fact]
    public void ResolveWithHash_Handles_Matching_PathBase()
    {
        var (cache, _) = Create();
        var result = cache.ResolveWithHash(new PathString("/app"), "/app/Scripts/site.js");
        Assert.StartsWith("/app/Scripts/site.js?v=", result);
    }

    [Fact]
    public void Constructor_Uses_Explicit_Https_Url()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.IsHttps = true;
        var accessor = new MockHttpContextAccessor { HttpContext = httpContext };
        var (cache, _) = Create(new ContentHashCache.CDNSettings
        {
            Enabled = true,
            Url = "http://cdn.example.com/",
            HttpsUrl = "https://secure.example.com/",
            Include = ["Scripts/*"]
        }, accessor);

        var result = cache.ResolveWithHash(PathString.Empty, "~/Scripts/site.js");
        Assert.StartsWith("https://secure.example.com/", result);
    }

    private static (ContentHashCache cache, MockHostEnvironment env) Create(
        ContentHashCache.CDNSettings? cdn = null, IHttpContextAccessor? accessor = null)
    {
        var env = new MockHostEnvironment();
        env.AddWebFile("Scripts/site.js", "content");
        var options = Microsoft.Extensions.Options.Options.Create(cdn ?? new ContentHashCache.CDNSettings());
        return (new ContentHashCache(options, env, accessor), env);
    }

    [Fact]
    public void Constructor_Throws_When_HostEnvironment_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ContentHashCache(Microsoft.Extensions.Options.Options.Create(new ContentHashCache.CDNSettings()), null!));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ResolvePath_Throws_For_Empty_ContentPath(string? contentPath)
    {
        var (cache, _) = Create();
        Assert.Throws<ArgumentNullException>(() => cache.ResolvePath(PathString.Empty, contentPath!));
    }

    [Fact]
    public void ResolvePath_Returns_Relative_Path_As_Is()
    {
        var (cache, _) = Create();
        Assert.Equal("Scripts/site.js", cache.ResolvePath(PathString.Empty, "Scripts/site.js"));
    }

    [Fact]
    public void ResolvePath_Resolves_App_Relative_Path()
    {
        var (cache, _) = Create();
        Assert.Equal("/Scripts/site.js", cache.ResolvePath(PathString.Empty, "~/Scripts/site.js"));
    }

    [Fact]
    public void ResolvePath_Applies_PathBase()
    {
        var (cache, _) = Create();
        Assert.Equal("/app/Scripts/site.js", cache.ResolvePath(new PathString("/app"), "~/Scripts/site.js"));
    }

    [Fact]
    public void ResolvePath_Uses_Cdn_When_Enabled()
    {
        var (cache, _) = Create(new ContentHashCache.CDNSettings
        {
            Enabled = true,
            Url = "https://cdn.example.com/"
        });

        Assert.Equal("https://cdn.example.com/Scripts/site.js",
            cache.ResolvePath(PathString.Empty, "~/Scripts/site.js"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ResolveWithHash_Throws_For_Empty_ContentUrl(string? contentUrl)
    {
        var (cache, _) = Create();
        Assert.Throws<ArgumentNullException>(() => cache.ResolveWithHash(PathString.Empty, contentUrl!));
    }

    [Fact]
    public void ResolveWithHash_Resolves_App_Relative_Url_With_Hash()
    {
        var (cache, _) = Create();

        var result = cache.ResolveWithHash(PathString.Empty, "~/Scripts/site.js");

        Assert.StartsWith("/Scripts/site.js?v=", result);
    }

    [Fact]
    public void ResolveWithHash_Returns_Invalid_Tilde_Path_Via_ToAbsolute()
    {
        var (cache, _) = Create();
        Assert.Equal("~Scripts", cache.ResolveWithHash(PathString.Empty, "~Scripts"));
    }

    [Fact]
    public void ResolveWithHash_Returns_Absolute_Url_When_PathBase_Does_Not_Match()
    {
        var (cache, _) = Create();
        Assert.Equal("/other.js", cache.ResolveWithHash(new PathString("/app"), "/other.js"));
    }

    [Fact]
    public void ResolveWithHash_Handles_Axd_When_Cdn_Disabled()
    {
        var (cache, _) = Create();
        Assert.Equal("/DynJS.axd/test",
            cache.ResolveWithHash(PathString.Empty, "~/DynJS.axd/test"));
    }

    [Fact]
    public void ResolveWithHash_Handles_Axd_With_Cdn_Enabled()
    {
        var (cache, _) = Create(new ContentHashCache.CDNSettings
        {
            Enabled = true,
            Url = "https://cdn.example.com/"
        });

        var result = cache.ResolveWithHash(PathString.Empty, "~/DynJS.axd/test");

        Assert.StartsWith("https://cdn.example.com/", result);
    }

    [Fact]
    public void ResolveWithHash_Applies_Cdn_With_Include_Filter()
    {
        var (cache, _) = Create(new ContentHashCache.CDNSettings
        {
            Enabled = true,
            Url = "https://cdn.example.com/",
            Include = ["Scripts/*"]
        });

        var result = cache.ResolveWithHash(PathString.Empty, "~/Scripts/site.js");

        Assert.StartsWith("https://cdn.example.com/", result);
    }

    [Fact]
    public void ResolveWithHash_Keeps_Local_When_Cdn_Exclude_Matches()
    {
        var (cache, _) = Create(new ContentHashCache.CDNSettings
        {
            Enabled = true,
            Url = "https://cdn.example.com/",
            Exclude = ["Scripts/*"]
        });

        var result = cache.ResolveWithHash(PathString.Empty, "~/Scripts/site.js");

        Assert.StartsWith("/Scripts/site.js?v=", result);
    }

    [Fact]
    public void ResolveWithHash_Uses_Https_Cdn_For_Secure_Requests()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.IsHttps = true;
        var accessor = new MockHttpContextAccessor { HttpContext = httpContext };
        var (cache, _) = Create(new ContentHashCache.CDNSettings
        {
            Enabled = true,
            Url = "http://cdn.example.com/",
            Include = ["Scripts/*"]
        }, accessor);

        var result = cache.ResolveWithHash(PathString.Empty, "~/Scripts/site.js");

        Assert.StartsWith("https://cdn.example.com/", result);
    }

    [Fact]
    public void ResolvePath_Returns_Short_Tilde_As_Is()
    {
        var (cache, _) = Create();
        Assert.Equal("~", cache.ResolvePath(PathString.Empty, "~"));
    }

    [Fact]
    public void ResolveWithHash_Returns_Short_Slash_As_Is()
    {
        var (cache, _) = Create();
        Assert.Equal("/", cache.ResolveWithHash(PathString.Empty, "/"));
    }

    [Fact]
    public void ResolveWithHash_Caches_Hash_Until_ScriptsChanged()
    {
        var env = new MockHostEnvironment();
        env.AddWebFile("Scripts/site.js", "content");
        var cache = new ContentHashCache(
            Microsoft.Extensions.Options.Options.Create(new ContentHashCache.CDNSettings()), env);

        var first = cache.ResolveWithHash(PathString.Empty, "~/Scripts/site.js");
        var second = cache.ResolveWithHash(PathString.Empty, "~/Scripts/site.js");
        Assert.Equal(first, second);

        env.File.WriteAllText(env.Path.Combine(env.WebRootPath, "Scripts/site.js"), "changed content");
        cache.ScriptsChanged();
        var third = cache.ResolveWithHash(PathString.Empty, "~/Scripts/site.js");

        Assert.NotEqual(first, third);
    }

    [Fact]
    public void ResolveWithHash_Uses_Timestamp_For_Missing_File()
    {
        var (cache, _) = Create();
        var first = cache.ResolveWithHash(PathString.Empty, "~/Scripts/missing.js");
        Assert.StartsWith("/Scripts/missing.js?v=", first);
    }

    [Fact]
    public void ScriptsChanged_Clears_Cache()
    {
        var (cache, _) = Create();
        var first = cache.ResolveWithHash(PathString.Empty, "~/Scripts/site.js");

        cache.ScriptsChanged();

        var second = cache.ResolveWithHash(PathString.Empty, "~/Scripts/site.js");
        Assert.Equal(first, second);
    }
}
