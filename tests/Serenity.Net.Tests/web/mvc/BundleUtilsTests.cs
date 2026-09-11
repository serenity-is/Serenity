namespace Serenity.Web;

public class BundleUtilsTests
{
    public BundleUtilsTests()
    {
        BundleUtils.ClearVersionCache();
    }

    private static MockHostEnvironment CreateEnvironment()
    {
        var env = new MockHostEnvironment();
        env.AddWebFile("Scripts/jquery-1.0.js", "a");
        env.AddWebFile("Scripts/jquery-1.2.js", "b");
        env.AddWebFile("Scripts/jquery-1.9.js", "c");
        return env;
    }

    [Fact]
    public void GetLatestVersion_Throws_For_Null_Path_Or_Pattern()
    {
        var env = CreateEnvironment();
        Assert.Throws<ArgumentNullException>(() =>
            BundleUtils.GetLatestVersion(env.WebRootFileProvider, null!, "x"));
        Assert.Throws<ArgumentNullException>(() =>
            BundleUtils.GetLatestVersion(env.WebRootFileProvider, "Scripts", null!));
    }

    [Fact]
    public void GetLatestVersion_Returns_Highest_Version()
    {
        var env = CreateEnvironment();

        var latest = BundleUtils.GetLatestVersion(env.WebRootFileProvider, "Scripts",
            @"^jquery-([0-9]?(\.[0-9])*)\.js$");

        Assert.Equal("jquery-1.9.js", latest);
    }

    [Fact]
    public void GetLatestVersion_Returns_Null_When_No_Match()
    {
        var env = CreateEnvironment();

        Assert.Null(BundleUtils.GetLatestVersion(env.WebRootFileProvider, "Scripts", @"^other-(.*)\.js$"));
    }

    [Fact]
    public void ExpandVersionVariable_Returns_Same_For_Null_Or_No_Version()
    {
        var env = CreateEnvironment();

        Assert.Null(BundleUtils.ExpandVersionVariable(env.WebRootFileProvider, null!));
        Assert.Equal("", BundleUtils.ExpandVersionVariable(env.WebRootFileProvider, ""));
        Assert.Equal("~/Scripts/site.js",
            BundleUtils.ExpandVersionVariable(env.WebRootFileProvider, "~/Scripts/site.js"));
    }

    [Fact]
    public void ExpandVersionVariable_Replaces_Version_And_Caches()
    {
        var env = CreateEnvironment();

        var result = BundleUtils.ExpandVersionVariable(env.WebRootFileProvider, "~/Scripts/jquery-{version}.js");
        Assert.Equal("~/Scripts/jquery-1.9.js", result);

        Assert.Equal(result,
            BundleUtils.ExpandVersionVariable(env.WebRootFileProvider, "~/Scripts/jquery-{version}.js"));
    }

    [Fact]
    public void ExpandVersionVariable_Returns_Original_When_No_Match()
    {
        var env = CreateEnvironment();

        Assert.Equal("~/Scripts/missing-{version}.js",
            BundleUtils.ExpandVersionVariable(env.WebRootFileProvider, "~/Scripts/missing-{version}.js"));
    }

    [Fact]
    public void DoReplacements_Replaces_Values()
    {
        Assert.Equal("aXc", BundleUtils.DoReplacements("a{b}c", new Dictionary<string, object>
        {
            ["b"] = "X"
        }));

        Assert.Equal("5", BundleUtils.DoReplacements("{count}", new Dictionary<string, object>
        {
            ["count"] = 5
        }));
    }

    [Fact]
    public void DoReplacements_Skips_Version_Placeholder()
    {
        Assert.Equal("a{version}b", BundleUtils.DoReplacements("a{version}b", []));
    }

    [Fact]
    public void DoReplacements_Returns_Null_For_Empty_Placeholder()
    {
        Assert.Null(BundleUtils.DoReplacements("a{}b", []));
    }

    [Fact]
    public void DoReplacements_Returns_Null_For_Missing_Value()
    {
        Assert.Null(BundleUtils.DoReplacements("a{b}c", []));
    }

    [Theory]
    [InlineData(true, null)]
    [InlineData(false, "ax")]
    public void DoReplacements_Falsey_Placeholder(bool value, string? expected)
    {
        var result = BundleUtils.DoReplacements("a{!flag}x", new Dictionary<string, object> { ["flag"] = value });
        Assert.Equal(expected, result);
    }

    [Fact]
    public void DoReplacements_Falsey_Placeholder_Returns_Null_For_NonNull_Non_Bool_Value()
    {
        Assert.Null(BundleUtils.DoReplacements("a{!flag}x", new Dictionary<string, object> { ["flag"] = "text" }));
    }

    [Fact]
    public void DoReplacements_Falsey_Placeholder_Missing_Value_Replaces_With_Empty()
    {
        Assert.Equal("ax", BundleUtils.DoReplacements("a{!flag}x", []));
    }

    [Fact]
    public void DoReplacements_Bool_True_Replaces_With_Empty_And_False_Returns_Null()
    {
        Assert.Equal("ax", BundleUtils.DoReplacements("a{flag}x", new Dictionary<string, object> { ["flag"] = true }));
        Assert.Null(BundleUtils.DoReplacements("a{flag}x", new Dictionary<string, object> { ["flag"] = false }));
    }

    [Fact]
    public void DoReplacements_Returns_Null_When_Replacements_Null()
    {
        Assert.Null(BundleUtils.DoReplacements("a{b}c", null!));
    }

    [Fact]
    public void DoReplacements_Returns_Same_When_No_Placeholder()
    {
        Assert.Equal("abc", BundleUtils.DoReplacements("abc", []));
    }

    [Fact]
    public void ExpandBundleIncludes_Expands_Nested_Bundles()
    {
        var bundles = new Dictionary<string, string[]>
        {
            ["main"] = ["a.js", "dynamic://Bundle.sub"],
            ["sub"] = ["b.js"],
            ["empty"] = [],
            ["null"] = null!,
            ["blank"] = ["", " "]
        };

        var result = BundleUtils.ExpandBundleIncludes(bundles, "dynamic://Bundle.", "script");

        Assert.Equal(["a.js", "b.js"], result["main"]);
        Assert.Equal(["b.js"], result["sub"]);
        Assert.Empty(result["empty"]);
        Assert.Empty(result["null"]);
        Assert.Equal([" "], result["blank"]);
    }

    [Fact]
    public void ExpandBundleIncludes_Throws_On_Infinite_Recursion()
    {
        var bundles = new Dictionary<string, string[]>
        {
            ["a"] = ["dynamic://Bundle.b"],
            ["b"] = ["dynamic://Bundle.a"]
        };

        Assert.Throws<InvalidOperationException>(() =>
            BundleUtils.ExpandBundleIncludes(bundles, "dynamic://Bundle.", "script"));
    }
}
