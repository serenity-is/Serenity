namespace Serenity.Web;

public class StaticAssetPatternFilterTests
{
    // the same defaults Serenity.Net.Web.StaticAssets.targets uses, with some absolute / project root patterns
    // from DefaultItemExcludes that can't match files under wwwroot
    private const string DefaultExcludes = @"bin\**;obj\**;P:\artifacts/**;**/*.user;**/*.*proj;**/.DS_Store;**/.*/**;" +
        @"**\node_modules\**;wwwroot\**\.*;wwwroot\**\*.tmp;wwwroot\**\*~;wwwroot\**\Thumbs.db;wwwroot\**\desktop.ini";

    [Theory]
    [InlineData("index.js", true)]
    [InlineData("css/site.css", true)]
    [InlineData("deep/er/path/file.js", true)]
    [InlineData("file.tmp", false)]
    [InlineData("sub/file.tmp", false)]
    [InlineData(".gitkeep", false)]
    [InlineData("sub/.hidden", false)]
    [InlineData(".hiddendir/file.js", false)]
    [InlineData("node_modules/pkg/index.js", false)]
    [InlineData("sub/node_modules/pkg/index.js", false)]
    [InlineData("my.user", false)]
    [InlineData("Thumbs.db", false)]
    [InlineData("backup.js~", false)]
    public void Applies_Default_Patterns(string path, bool expected)
    {
        var filter = new StaticAssetPatternFilter(@"wwwroot\**", DefaultExcludes);
        Assert.Equal(expected, filter.IsMatch(path));
    }

    [Fact]
    public void Ignores_Patterns_Outside_The_Folder()
    {
        // bin\**, obj\**, absolute paths and root level patterns can't match files under wwwroot
        var filter = new StaticAssetPatternFilter(@"wwwroot\**", @"bin\**;obj\**;P:\artifacts/**;*.js");
        Assert.True(filter.IsMatch("bin/file.js"));
        Assert.True(filter.IsMatch("obj/file.js"));
        Assert.True(filter.IsMatch("file.js"));
    }

    [Theory]
    [InlineData(@"css\site.css", true)]
    [InlineData("/css/site.css", true)]
    [InlineData("CSS/SITE.CSS", true)]
    public void Normalizes_Separators_Slashes_And_Case(string path, bool expected)
    {
        var filter = new StaticAssetPatternFilter(@"wwwroot\css\**", null);
        Assert.Equal(expected, filter.IsMatch(path));
    }

    [Theory]
    [InlineData("esm/page.js", true)]
    [InlineData("esm/sub/page.js", true)]
    [InlineData("index.css", true)]
    [InlineData("index.js", false)]
    [InlineData("other/style.css", false)]
    public void Uses_Whitelist_Includes(string path, bool expected)
    {
        var filter = new StaticAssetPatternFilter(@"wwwroot\esm\**;wwwroot\*.css", DefaultExcludes);
        Assert.Equal(expected, filter.IsMatch(path));
    }

    [Fact]
    public void Includes_All_Files_When_There_Are_No_Includes()
    {
        var filter = new StaticAssetPatternFilter(null, @"wwwroot\**\*.map");
        Assert.True(filter.IsMatch("index.js"));
        Assert.False(filter.IsMatch("index.js.map"));
    }

    [Fact]
    public void Applies_Custom_Excludes()
    {
        var filter = new StaticAssetPatternFilter(@"wwwroot\**", DefaultExcludes + @";wwwroot\**\*.map;wwwroot\docs\**");
        Assert.True(filter.IsMatch("index.js"));
        Assert.False(filter.IsMatch("index.js.map"));
        Assert.False(filter.IsMatch("docs/readme.html"));
    }

    [Fact]
    public void Keeps_WellKnown_Folder_When_Enabled()
    {
        var filter = new StaticAssetPatternFilter(@"wwwroot\**", DefaultExcludes, wellKnown: true,
            wellKnownExcludes: @"bin\**;**/*.user;wwwroot\**\secret.txt");
        Assert.True(filter.IsMatch(".well-known/security.txt"));
        Assert.False(filter.IsMatch(".well-known/my.user"));
        Assert.False(filter.IsMatch(".well-known/secret.txt"));
        // other dot folders are still excluded
        Assert.False(filter.IsMatch(".other/security.txt"));
    }

    [Fact]
    public void Excludes_WellKnown_Folder_When_Not_Enabled()
    {
        var filter = new StaticAssetPatternFilter(@"wwwroot\**", DefaultExcludes);
        Assert.False(filter.IsMatch(".well-known/security.txt"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("/")]
    public void Does_Not_Match_Empty_Paths(string? path)
    {
        var filter = new StaticAssetPatternFilter(null, null);
        Assert.False(filter.IsMatch(path!));
    }

    [Fact]
    public void Supports_A_Custom_Folder()
    {
        var filter = new StaticAssetPatternFilter(@"content\**", @"content\**\*.tmp", folder: "content");
        Assert.True(filter.IsMatch("site.css"));
        Assert.False(filter.IsMatch("site.tmp"));
    }
}
