namespace Serenity.Web.EsBuild;

public class EsBuildMinifierTests
{
    private sealed class FakeCLI : IEsBuildCLI
    {
        public Func<string, int, string>? Css { get; set; }
        public Func<string, int, string>? Script { get; set; }
        public int Calls { get; private set; }
        public int? LastCssLineLimit { get; private set; }
        public int? LastScriptLineLimit { get; private set; }

        public string MinifyCss(string code, int lineLimit = 1000)
        {
            Calls++;
            LastCssLineLimit = lineLimit;
            return Css?.Invoke(code, lineLimit) ?? "CSS";
        }

        public string MinifyScript(string code, int lineLimit = 1000)
        {
            Calls++;
            LastScriptLineLimit = lineLimit;
            return Script?.Invoke(code, lineLimit) ?? "JS";
        }
    }

    [Fact]
    public void MinifyCss_Returns_CLI_Output()
    {
        var cli = new FakeCLI();
        var minifier = new EsBuildMinifier(cliFactory: () => cli);

        var result = minifier.MinifyCss("body{}", new CssMinifyOptions());

        Assert.Equal("CSS", result.Code);
        Assert.False(result.HasErrors);
        Assert.Equal(int.MaxValue - 1000, cli.LastCssLineLimit);
    }

    [Fact]
    public void MinifyCss_Uses_LineBreakThreshold()
    {
        var cli = new FakeCLI();
        var minifier = new EsBuildMinifier(cliFactory: () => cli);

        minifier.MinifyCss("body{}", new CssMinifyOptions { LineBreakThreshold = 20 });

        Assert.Equal(20, cli.LastCssLineLimit);
    }

    [Fact]
    public void MinifyCss_Returns_Original_On_Error()
    {
        var cli = new FakeCLI { Css = (_, _) => throw new InvalidOperationException("boom") };
        var minifier = new EsBuildMinifier(cliFactory: () => cli);

        var result = minifier.MinifyCss("body{}", new CssMinifyOptions());

        Assert.True(result.HasErrors);
        Assert.Equal("body{}", result.Code);
    }

    [Fact]
    public void MinifyScript_Returns_CLI_Output()
    {
        var cli = new FakeCLI();
        var minifier = new EsBuildMinifier(cliFactory: () => cli);

        var result = minifier.MinifyScript("var a = 1;", new ScriptMinifyOptions { LineBreakThreshold = 30 });

        Assert.Equal("JS", result.Code);
        Assert.False(result.HasErrors);
        Assert.Equal(30, cli.LastScriptLineLimit);
    }

    [Fact]
    public void MinifyScript_Returns_Original_On_Error()
    {
        var cli = new FakeCLI { Script = (_, _) => throw new InvalidOperationException("boom") };
        var minifier = new EsBuildMinifier(cliFactory: () => cli);

        var result = minifier.MinifyScript("var a = 1;", new ScriptMinifyOptions());

        Assert.True(result.HasErrors);
        Assert.Equal("var a = 1;", result.Code);
    }

    [Fact]
    public void CLI_Is_Created_Once()
    {
        int factoryCalls = 0;
        var cli = new FakeCLI();
        var minifier = new EsBuildMinifier(cliFactory: () => { factoryCalls++; return cli; });

        minifier.MinifyCss("a", new CssMinifyOptions());
        minifier.MinifyScript("b", new ScriptMinifyOptions());

        Assert.Equal(1, factoryCalls);
    }
}
