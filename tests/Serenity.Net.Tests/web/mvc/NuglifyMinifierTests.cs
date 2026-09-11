namespace Serenity.Web;

public class NuglifyMinifierTests
{
    [Fact]
    public void MinifyCss_Minifies_Content()
    {
        var result = new NuglifyMinifier().MinifyCss("body { color: red; }", new CssMinifyOptions());

        Assert.False(result.HasErrors);
        Assert.Contains("color", result.Code);
    }

    [Fact]
    public void MinifyCss_With_LineBreakThreshold()
    {
        var result = new NuglifyMinifier().MinifyCss("body { color: red; }", new CssMinifyOptions
        {
            LineBreakThreshold = 10
        });

        Assert.False(result.HasErrors);
    }

    [Fact]
    public void MinifyScript_Minifies_Content()
    {
        var result = new NuglifyMinifier().MinifyScript("var a = 1; var b = 2;", new ScriptMinifyOptions());

        Assert.False(result.HasErrors);
        Assert.Contains("a", result.Code);
    }

    [Fact]
    public void MinifyScript_With_LineBreakThreshold()
    {
        var result = new NuglifyMinifier().MinifyScript("var a = 1;", new ScriptMinifyOptions
        {
            LineBreakThreshold = 10
        });

        Assert.False(result.HasErrors);
    }
}
