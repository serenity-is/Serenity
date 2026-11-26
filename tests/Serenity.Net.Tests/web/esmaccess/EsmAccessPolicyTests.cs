namespace Serenity.Web;

public class EsmAccessPolicyTests
{
    private const string Login = "Modules/Membership/Account/Login/LoginPage.js";
    private const string User = "Modules/Administration/User/UserPage.js";
    private const string Order = "Modules/Northwind/Order/OrderPage.js";

    // the login page shares a chunk with the user page, the user and order pages share another
    private static readonly string ManifestJson = JSON.Stringify(new
    {
        version = 1,
        entries = new[] { Login, User, Order },
        files = new Dictionary<string, int[]>
        {
            [Login] = [0],
            [User] = [1],
            [User + ".map"] = [1],
            [Order] = [2],
            ["_chunks/chunk-LOGIN.js"] = [0, 1],
            ["_chunks/chunk-PAGES.js"] = [1, 2],
            ["_chunks/chunk-ORDER.js"] = [2],
            ["assets/font.woff2"] = []
        }
    });

    private class TestLayoutScripts(Dictionary<string, string?> modules) : ILayoutScriptFileProvider
    {
        public IReadOnlyList<string> GetFiles() => [];
        public IReadOnlyList<string> GetAllFiles(bool module) => module ? [.. modules.Keys] : [];
        public string? GetPermission(string path) => modules.TryGetValue(path, out var p) ? p : null;
    }

    private static (EsmAccessPolicy policy, MockHostEnvironment env) Create(Action<EsmAccessControlOptions>? configure = null,
        Dictionary<string, string?>? layoutModules = null, string? manifest = null, string root = "esm/")
    {
        var env = new MockHostEnvironment();
        env.AddWebFile(root + EsmManifest.FileName, manifest ?? ManifestJson);
        var options = new EsmAccessControlOptions { Enabled = true };
        configure?.Invoke(options);
        var policy = new EsmAccessPolicy(env.WebRootFileProvider, options,
            layoutModules is null ? null : new TestLayoutScripts(layoutModules));
        return (policy, env);
    }

    private static void AssertPublic(EsmFileAccess access) => Assert.Equal(EsmFileAccessKind.Public, access.Kind);

    private static void AssertRestricted(EsmFileAccess access, params string[] permissions)
    {
        Assert.Equal(EsmFileAccessKind.Restricted, access.Kind);
        Assert.Equal(permissions.OrderBy(x => x), access.Permissions.OrderBy(x => x));
    }

    [Fact]
    public void Level1_Anonymous_Entries_And_The_Files_They_Reach_Are_Public()
    {
        var (policy, _) = Create();
        AssertPublic(policy.GetAccess("/esm/" + Login));
        AssertPublic(policy.GetAccess("/esm/_chunks/chunk-LOGIN.js"));
    }

    [Fact]
    public void Level1_Other_Files_Require_Default_Permission()
    {
        var (policy, _) = Create();
        AssertRestricted(policy.GetAccess("/esm/" + User), "?");
        AssertRestricted(policy.GetAccess("/esm/" + User + ".map"), "?");
        AssertRestricted(policy.GetAccess("/esm/_chunks/chunk-PAGES.js"), "?");
        AssertRestricted(policy.GetAccess("/esm/_chunks/chunk-ORDER.js"), "?");
    }

    [Fact]
    public void Files_No_Entry_Reaches_Are_Public()
    {
        var (policy, _) = Create();
        AssertPublic(policy.GetAccess("/esm/assets/font.woff2"));
    }

    [Fact]
    public void Level2_Uses_Longest_Matching_Pattern_And_Combines_Entries_Of_A_Chunk()
    {
        var (policy, _) = Create(o => o.Permissions = new()
        {
            ["Modules/**"] = "General",
            ["Modules/Administration/**"] = "Administration:General"
        });

        AssertRestricted(policy.GetAccess("/esm/" + User), "Administration:General");
        AssertRestricted(policy.GetAccess("/esm/" + Order), "General");
        AssertRestricted(policy.GetAccess("/esm/_chunks/chunk-PAGES.js"), "Administration:General", "General");
        // anonymous paths win over the permission patterns
        AssertPublic(policy.GetAccess("/esm/" + Login));
        AssertPublic(policy.GetAccess("/esm/_chunks/chunk-LOGIN.js"));
    }

    [Fact]
    public void Level2_Unmatched_Entries_Use_Default_Permission()
    {
        var (policy, _) = Create(o =>
        {
            o.Permissions = new() { ["Modules/Administration/**"] = "Administration:General" };
            o.DefaultPermission = "Default:Key";
        });

        AssertRestricted(policy.GetAccess("/esm/" + Order), "Default:Key");
    }

    [Fact]
    public void Layout_Modules_Use_Their_Own_Permission()
    {
        var (policy, _) = Create(layoutModules: new()
        {
            ["~/esm/" + Order] = "*",
            ["~/esm/" + User] = "Administration:Security"
        });

        AssertPublic(policy.GetAccess("/esm/" + Order));
        AssertPublic(policy.GetAccess("/esm/_chunks/chunk-ORDER.js"));
        // reached by a public layout module, so public
        AssertPublic(policy.GetAccess("/esm/_chunks/chunk-PAGES.js"));
        AssertRestricted(policy.GetAccess("/esm/" + User), "Administration:Security");
    }

    [Fact]
    public void Manifest_Is_Hidden()
    {
        var (policy, _) = Create();
        Assert.Equal(EsmFileAccessKind.Hidden, policy.GetAccess("/esm/" + EsmManifest.FileName).Kind);
        Assert.Equal(EsmFileAccessKind.Hidden, policy.GetAccess("/esm/BUILD.ESM-MANIFEST").Kind);
    }

    [Fact]
    public void Manifest_Has_No_Content_Type_So_Static_Files_Never_Serve_It()
    {
        // even when the access control is disabled, as static files return 404 for unknown file types
        Assert.False(new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider()
            .TryGetContentType(EsmManifest.FileName, out _));
    }

    [Fact]
    public void Files_Not_In_Manifest_Are_Entries_Of_Their_Own()
    {
        var (policy, _) = Create();
        AssertPublic(policy.GetAccess("/esm/bundles/script.bundle.js"));
        AssertRestricted(policy.GetAccess("/esm/Modules/Unknown.js"), "?");
    }

    [Fact]
    public void Precompressed_Variants_Have_The_Access_Of_The_File()
    {
        var (policy, _) = Create();
        AssertRestricted(policy.GetAccess("/esm/" + User + ".gz"), "?");
        AssertRestricted(policy.GetAccess("/esm/" + User + ".br"), "?");
        AssertPublic(policy.GetAccess("/esm/" + Login + ".br"));
    }

    [Theory]
    [InlineData("/esm/Modules\\Administration\\User\\UserPage.js")]
    [InlineData("/esm//Modules/Administration/User/UserPage.js")]
    [InlineData("/ESM/modules/administration/user/userpage.js")]
    public void Path_Variants_Are_Normalized(string path)
    {
        var (policy, _) = Create(o => o.Permissions = new() { ["Modules/Administration/**"] = "Administration:General" });
        AssertRestricted(policy.GetAccess(path), "Administration:General");
    }

    [Fact]
    public void Paths_Outside_Esm_Folders_Are_Not_Handled()
    {
        var (policy, _) = Create();
        Assert.Equal(EsmFileAccessKind.NotHandled, policy.GetAccess("/Content/site.css").Kind);
        Assert.Equal(EsmFileAccessKind.NotHandled, policy.GetAccess("/esm/").Kind);
        Assert.Equal(EsmFileAccessKind.NotHandled, policy.GetAccess(null).Kind);
    }

    [Fact]
    public void Esm_Folder_Without_Manifest_Is_Not_Handled()
    {
        var (policy, _) = Create();
        Assert.Equal(EsmFileAccessKind.NotHandled, policy.GetAccess("/Some.Package/esm/" + User).Kind);
    }

    [Fact]
    public void Package_Esm_Folder_Uses_Its_Own_Manifest()
    {
        var (policy, _) = Create(root: "Serenity.Extensions/esm/");
        AssertPublic(policy.GetAccess("/Serenity.Extensions/esm/" + Login));
        AssertRestricted(policy.GetAccess("/Serenity.Extensions/esm/" + User), "?");
        Assert.Equal(EsmFileAccessKind.NotHandled, policy.GetAccess("/esm/" + User).Kind);
    }

    [Fact]
    public void Unreadable_Manifest_Denies_By_Default()
    {
        var (policy, _) = Create(manifest: "not json");
        AssertRestricted(policy.GetAccess("/esm/_chunks/chunk-LOGIN.js"), "?");
        AssertPublic(policy.GetAccess("/esm/" + Login));
    }

    [Fact]
    public void Changed_Manifest_Is_Reloaded()
    {
        var (policy, env) = Create();
        policy.ManifestCheckInterval = TimeSpan.Zero;
        AssertRestricted(policy.GetAccess("/esm/_chunks/chunk-NEW.js"), "?");

        env.AddWebFile("esm/" + EsmManifest.FileName, JSON.Stringify(new
        {
            version = 1,
            entries = new[] { Login },
            files = new Dictionary<string, int[]> { ["_chunks/chunk-NEW.js"] = [0] }
        }));
        env.FileSystem.File.SetLastWriteTimeUtc(env.FileSystem.Path.Combine(env.WebRootPath, "esm", EsmManifest.FileName),
            DateTime.UtcNow.AddMinutes(1));

        AssertPublic(policy.GetAccess("/esm/_chunks/chunk-NEW.js"));
    }
}
