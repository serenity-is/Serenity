namespace Serenity.Web;

public class HtmlLazyTypesExtensionsTests
{
    private static string Render(params Attribute[] attributes)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IContentHashCache>(new MockContentHashCache
        {
            ResolveWithHashCallback = (pathBase, url) => "/resolved" + url[1..] + "?v=hash"
        });
        services.AddSingleton<ITypeSource>(new MockTypeSource(attributes));
        return MockHtmlHelper.Create(services.BuildServiceProvider()).LazyTypes().Value!;
    }

    [Fact]
    public void LazyTypes_Throws_For_Null_Helper()
    {
        Assert.Throws<ArgumentNullException>(() => HtmlLazyTypesExtensions.LazyTypes(null!));
    }

    [Fact]
    public void LazyTypes_Maps_The_Content_Hashed_Module_Urls_To_Their_Keys()
    {
        var result = Render(
            new LazyLoadTypesAttribute("~/a/esm/LazyTypeLoader.js", "A.One", "A.Two"),
            new LazyLoadTypesAttribute("~/b/esm/LazyTypeLoader.js", "B.One"));

        Assert.StartsWith("<script type=\"application/json\" id=\"LazyTypes\">", result);
        var json = result[(result.IndexOf('>') + 1)..result.LastIndexOf("</script>", StringComparison.Ordinal)];
        var modules = JSON.Parse<Dictionary<string, string[]>>(json);

        Assert.Equal(2, modules.Count);
        Assert.Equal(["A.One", "A.Two"], modules["/resolved/a/esm/LazyTypeLoader.js?v=hash"]);
        Assert.Equal(["B.One"], modules["/resolved/b/esm/LazyTypeLoader.js?v=hash"]);
    }

    [Fact]
    public void LazyTypes_Merges_The_Keys_Of_The_Same_Module()
    {
        var result = Render(
            new LazyLoadTypesAttribute("~/a.js", "One"),
            new LazyLoadTypesAttribute("~/a.js", "Two"));

        Assert.Contains("[\"One\",\"Two\"]", result);
    }

    [Fact]
    public void LazyTypes_Renders_Nothing_Without_Types()
    {
        Assert.Equal("", Render());
        Assert.Equal("", Render(new LazyLoadTypesAttribute("~/a.js")));
    }

    [Fact]
    public void LazyTypes_Renders_Nothing_Without_A_Type_Source()
    {
        Assert.Equal("", MockHtmlHelper.Create(new ServiceCollection().BuildServiceProvider()).LazyTypes().Value);
    }

    [Fact]
    public void LazyLoadTypesAttribute_Throws_For_Null_Arguments()
    {
        Assert.Throws<ArgumentNullException>(() => new LazyLoadTypesAttribute(null!, "One"));
        Assert.Throws<ArgumentNullException>(() => new LazyLoadTypesAttribute("~/a.js", null!));
    }
}
