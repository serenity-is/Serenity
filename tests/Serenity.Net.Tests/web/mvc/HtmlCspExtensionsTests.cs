namespace Serenity.Web;

public class HtmlCspExtensionsTests
{
    private class TestController : ControllerBase
    {
    }

    [Fact]
    public void CspNonce_Throws_When_Html_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(() => HtmlCspExtensions.CspNonce(null!));
    }

    [Fact]
    public void CspNonce_Generates_And_Caches_Nonce_And_Adds_Directives()
    {
        var html = MockHtmlHelper.Create();

        var first = html.CspNonce();
        var second = html.CspNonce();

        Assert.NotNull(first);
        Assert.Equal(first, second);
        Assert.Contains("'nonce-" + first + "'", html.GetCspDirective("script-src").Value);
        Assert.Contains("'nonce-" + first + "'", html.GetCspDirective("style-src").Value);
    }

    [Fact]
    public void CspNonce_Does_Not_Add_Directives_When_False()
    {
        var html = MockHtmlHelper.Create();

        html.CspNonce(addDirectives: false);

        Assert.Equal("script-src ;", html.GetCspDirective("script-src").Value);
    }

    [Fact]
    public void AddCspDirective_Throws_When_Items_Null()
    {
        var method = typeof(HtmlCspExtensions).GetMethod("AddCspDirective",
            BindingFlags.NonPublic | BindingFlags.Static, null,
            [typeof(IDictionary<object, object?>), typeof(string), typeof(string[])], null)!;

        var exception = Assert.Throws<TargetInvocationException>(() =>
            method.Invoke(null, [null, "script-src", new[] { "self" }]));

        Assert.IsType<ArgumentNullException>(exception.InnerException);
    }

    [Fact]
    public void AddCspDirective_Throws_For_Disallowed_Directive()
    {
        var html = MockHtmlHelper.Create();
        Assert.Throws<ArgumentException>(() => html.AddCspDirective("unknown-directive", "x"));
    }

    [Fact]
    public void AddCspDirective_Throws_For_Null_Directive_Name()
    {
        var html = MockHtmlHelper.Create();
        Assert.Throws<ArgumentNullException>(() => html.AddCspDirective(null!, "x"));
    }

    [Fact]
    public void AddCspDirective_Auto_Quotes_Keywords()
    {
        var html = MockHtmlHelper.Create();

        html.AddCspDirective("script-src", "self", "'unsafe-inline'", "https://example.com", "");

        var directive = html.GetCspDirective("script-src").Value;
        Assert.Contains("'self'", directive);
        Assert.Contains("'unsafe-inline'", directive);
        Assert.Contains("https://example.com", directive);
    }

    [Fact]
    public void AddCspDirective_On_HttpContext_And_Controller()
    {
        var services = new ServiceCollection().BuildServiceProvider();
        HttpContext? captured = null;
        var html = MockHtmlHelper.Create(services, context => captured = context);

        captured!.AddCspDirective("img-src", "data");
        var controller = new TestController
        {
            ControllerContext = new ControllerContext { HttpContext = captured }
        };
        controller.AddCspDirective("img-src", "blob");

        var directive = html.GetCspDirective("img-src").Value;
        Assert.Contains("'data'", directive);
        Assert.Contains("'blob'", directive);
    }

    [Fact]
    public void GetCspDirective_Throws_For_Disallowed_Directive()
    {
        var html = MockHtmlHelper.Create();
        Assert.Throws<ArgumentException>(() => html.GetCspDirective("unknown"));
    }

    [Fact]
    public void GetCspDirective_Merges_Manual_And_Stored_Values()
    {
        var html = MockHtmlHelper.Create();
        html.AddCspDirective("script-src", "self");

        var directive = html.GetCspDirective("script-src", "https://example.com").Value;

        Assert.Contains("'self'", directive);
        Assert.Contains("https://example.com", directive);
    }

    [Fact]
    public void GetCspDirective_Removes_Nonces_When_UnsafeInline_Present()
    {
        var html = MockHtmlHelper.Create();
        var nonce = html.CspNonce();
        html.AddCspDirective("script-src", "'unsafe-inline'");

        var directive = html.GetCspDirective("script-src").Value;

        Assert.DoesNotContain("nonce-", directive);
        Assert.Contains("'unsafe-inline'", directive);
    }

    [Fact]
    public void AddCspScriptUrl_Adds_Directive_And_Returns_Url()
    {
        var html = MockHtmlHelper.Create();

        var result = html.AddCspScriptUrl("https://cdn.example.com/lib.js");

        Assert.Equal("https://cdn.example.com/lib.js", result);
        Assert.Contains("https://cdn.example.com/lib.js", html.GetCspDirective("script-src").Value);
    }
}
