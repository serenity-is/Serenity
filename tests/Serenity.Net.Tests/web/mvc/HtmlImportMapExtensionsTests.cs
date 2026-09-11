namespace Serenity.Web;

public class HtmlImportMapExtensionsTests
{
    private static MockContentHashCache ContentHashCache()
    {
        return new MockContentHashCache
        {
            ResolveWithHashCallback = (pathBase, url) => "/resolved" + url[1..]
        };
    }

    [Fact]
    public void AddImportMapEntry_Throws_For_Null_Arguments()
    {
        var context = new DefaultHttpContext();

        Assert.Throws<ArgumentNullException>(() =>
            HtmlImportMapExtensions.AddImportMapEntry(null!, "a", "b"));
        Assert.Throws<ArgumentNullException>(() =>
            context.AddImportMapEntry(null!, "b"));
        Assert.Throws<ArgumentNullException>(() =>
            context.AddImportMapEntry("a", null!));
    }

    [Fact]
    public void AddImportMapEntry_Resolves_App_Relative_Address()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IContentHashCache>(ContentHashCache());
        var context = new DefaultHttpContext { RequestServices = services.BuildServiceProvider() };

        context.AddImportMapEntry("mymod", "~/Scripts/mymod.js");

        var html = MockHtmlHelper.CreateWithItems(context.Items, context.RequestServices);
        var rendered = html.RenderImportMap().Value;

        Assert.Contains("\"mymod\": \"/resolved/Scripts/mymod.js\"", rendered);
    }

    [Fact]
    public void AddImportMapEntry_Does_Not_Overwrite_When_Disabled()
    {
        var context = new DefaultHttpContext();

        context.AddImportMapEntry("mymod", "a");
        context.AddImportMapEntry("mymod", "b", overwrite: false);

        var html = MockHtmlHelper.CreateWithItems(context.Items, context.RequestServices);
        Assert.Contains("\"mymod\": \"a\"", html.RenderImportMap().Value);
    }

    [Fact]
    public void AddImportMapEntry_Adds_Csp_For_Absolute_Addresses()
    {
        var context = new DefaultHttpContext();

        context.AddImportMapEntry("mymod", "https://cdn.example.com/mymod.js");

        var html = MockHtmlHelper.CreateWithItems(context.Items, context.RequestServices);
        Assert.Contains("https://cdn.example.com/mymod.js", html.GetCspDirective("script-src").Value);
    }

    [Fact]
    public void AddImportMapEntry_Stores_Integrity()
    {
        var context = new DefaultHttpContext();

        context.AddImportMapEntry("mymod", "a", integrity: "sha384-abc");

        var html = MockHtmlHelper.CreateWithItems(context.Items, context.RequestServices);
        Assert.Contains("\"a\": \"sha384-abc\"", html.RenderImportMap().Value);
    }

    [Fact]
    public void AddSerenityAssetsImportMapEntries_Adds_Entries()
    {
        var context = new DefaultHttpContext
        {
            RequestServices = new ServiceCollection().BuildServiceProvider()
        };

        context.AddSerenityAssetsImportMapEntries();

        var html = MockHtmlHelper.CreateWithItems(context.Items, context.RequestServices);
        var rendered = html.RenderImportMap().Value;
        Assert.Contains("jspdf", rendered);
        Assert.Contains("jspdf-autotable", rendered);
        Assert.Contains("@serenity-is/tiptap", rendered);
    }

    [Fact]
    public void RenderImportMap_Throws_For_Null_Helper()
    {
        Assert.Throws<ArgumentNullException>(() => HtmlImportMapExtensions.RenderImportMap(null!));
    }

    [Fact]
    public void RenderImportMap_Returns_Empty_Without_Entries()
    {
        var html = MockHtmlHelper.Create();
        Assert.Equal("", html.RenderImportMap().Value);
    }
}
