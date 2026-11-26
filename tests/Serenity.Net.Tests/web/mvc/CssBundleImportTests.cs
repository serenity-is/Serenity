using System.Diagnostics;
using System.Text;

namespace Serenity.Web;

public class CssBundleImportTests
{
    private static string BuildBundle(MockHostEnvironment env,
        Action<CssBundlingOptions>? configure = null, params string[] files)
    {
        var container = new ServiceCollection();
        container.AddSingleton<IWebHostEnvironment>(env);
        container.AddSingleton<IPermissionService, MockPermissions>();
        container.AddCssBundling(options =>
        {
            options.Enabled = true;
            options.Minimize = false;
            options.UseMinCSS = false;
            options.Bundles["Test"] = [.. files.Select(x => "~/" + x)];
            configure?.Invoke(options);
        });

        var services = container.BuildServiceProvider();
        var scriptManager = services.GetRequiredService<IDynamicScriptManager>();
        var cssBundleManager = services.GetRequiredService<ICssBundleManager>();
        foreach (var file in files)
            cssBundleManager.GetCssBundle("~/" + file);

        return scriptManager.GetScriptText("CssBundle.Test")?.Replace(";", "").Trim() ?? "";
    }

    private static string Normalize(string? text) =>
        (text ?? "").Replace("\r\n", "\n").Replace(" ", "").Replace("\n", "").Trim();

    [Fact]
    public void Import_At_The_Start_Of_A_File_Is_Embedded()
    {
        var env = new MockHostEnvironment();
        env.AddWebFile("a.css", "@import \"b.css\";\n.own{color:red}");
        env.AddWebFile("b.css", ".imported{color:blue}");

        var actual = Normalize(BuildBundle(env, files: ["a.css"]));

        Assert.Equal(".imported{color:blue}.own{color:red}", actual);
    }

    [Fact]
    public void Import_After_Comments_And_Whitespace_Is_Embedded()
    {
        var env = new MockHostEnvironment();
        env.AddWebFile("a.css", "/* header */\n\n/* second */\n@import 'b.css';\n.own{color:red}");
        env.AddWebFile("b.css", ".imported{color:blue}");

        var actual = Normalize(BuildBundle(env, files: ["a.css"]));

        Assert.Equal(".imported{color:blue}.own{color:red}", actual);
    }

    [Fact]
    public void Imports_Are_Embedded_In_Place_Without_Duplicating_The_File()
    {
        var env = new MockHostEnvironment();
        env.AddWebFile("a.css", "@import \"b.css\";\n@import \"c.css\";\n.own{color:red}");
        env.AddWebFile("b.css", ".first{color:blue}");
        env.AddWebFile("c.css", ".second{color:green}");

        var actual = Normalize(BuildBundle(env, files: ["a.css"]));

        Assert.Equal(".first{color:blue}.second{color:green}.own{color:red}", actual);
    }

    [Fact]
    public void Nested_Imports_Are_Embedded()
    {
        var env = new MockHostEnvironment();
        env.AddWebFile("a.css", "@import \"b.css\";\n.own{color:red}");
        env.AddWebFile("b.css", "@import \"sub/c.css\";\n.middle{color:blue}");
        env.AddWebFile("sub/c.css", ".deep{color:green}");

        var actual = Normalize(BuildBundle(env, files: ["a.css"]));

        Assert.Equal(".deep{color:green}.middle{color:blue}.own{color:red}", actual);
    }

    [Theory]
    [InlineData("@import \"b.css\" screen;\n.own{color:red}")]
    [InlineData("@import url(\"b.css\") print;\n.own{color:red}")]
    public void Media_Qualified_Imports_Are_Left_As_Is(string content)
    {
        var env = new MockHostEnvironment();
        env.AddWebFile("a.css", content);
        env.AddWebFile("b.css", ".imported{color:blue}");

        var actual = BuildBundle(env, files: ["a.css"]);

        Assert.Contains("@import", actual);
        Assert.DoesNotContain(".imported", actual);
        Assert.Contains(".own{color:red}", actual);
    }

    [Theory]
    [InlineData("@import url(\"https://fonts.example/x.css\");\n.own{color:red}")]
    [InlineData("@import url(\"//cdn.example/x.css\");\n.own{color:red}")]
    [InlineData("@import url(\"/absolute/x.css\");\n.own{color:red}")]
    [InlineData("@import url(\"data:text/css,a{};\");\n.own{color:red}")]
    public void External_Absolute_And_Data_Imports_Are_Left_As_Is(string content)
    {
        var env = new MockHostEnvironment();
        env.AddWebFile("a.css", content);

        var actual = BuildBundle(env, files: ["a.css"]);

        Assert.Contains("@import", actual);
        Assert.Contains(".own{color:red}", actual);
    }

    [Fact]
    public void Import_That_Is_Not_At_The_Start_Of_A_File_Is_Left_As_Is()
    {
        // browsers ignore @import after the first rule, so it is not processed
        var env = new MockHostEnvironment();
        env.AddWebFile("a.css", ".own{color:red}\n@import \"b.css\";");
        env.AddWebFile("b.css", ".imported{color:blue}");

        var actual = BuildBundle(env, files: ["a.css"]);

        Assert.Contains("@import", actual);
        Assert.DoesNotContain(".imported", actual);
    }

    [Fact]
    public void Import_Processing_Can_Be_Turned_Off()
    {
        var env = new MockHostEnvironment();
        env.AddWebFile("a.css", "@import \"b.css\";\n.own{color:red}");
        env.AddWebFile("b.css", ".imported{color:blue}");

        var actual = BuildBundle(env, options => options.ProcessImports = false, "a.css");

        Assert.Contains("@import", actual);
        Assert.DoesNotContain(".imported", actual);
    }

    [Fact]
    public void Comments_Containing_Stars_Are_Skipped_Before_Import()
    {
        var env = new MockHostEnvironment();
        env.AddWebFile("a.css", "/*! license ***/ /* *** */\n@import \"b.css\";\n.own{color:red}");
        env.AddWebFile("b.css", ".imported{color:blue}");

        var actual = Normalize(BuildBundle(env, files: ["a.css"]));

        Assert.Equal(".imported{color:blue}.own{color:red}", actual);
    }

    [Fact]
    public void Comments_Are_Not_Produced_For_Invalid_Import_Statements()
    {
        var env = new MockHostEnvironment();
        env.AddWebFile("a.css", "@import \"b.css\"\n.own{color:red}");
        env.AddWebFile("b.css", ".imported{color:blue}");

        var actual = BuildBundle(env, files: ["a.css"]);

        Assert.Contains(".own{color:red}", actual);
    }

    /// <summary>
    /// A file that starts with a comment, and has comments that directly follow
    /// another comment after its first rule, e.g. a section header above a note.
    /// The previous import pattern could end the leading comment at any later
    /// <c>*/</c> and go on from there when that one was followed by a comment,
    /// so its time doubled with every such pair: 24 pairs took ~1.5 seconds,
    /// 28 pairs over 20 seconds, and a real stylesheet with 28 of them (57 KB,
    /// 78 comments) never finished, which hung the CSS bundle request.
    /// </summary>
    private static string CommentPairsAfterFirstRule(int pairs, string? ending = null)
    {
        var sb = new StringBuilder();
        sb.Append("/*\n * header comment\n */\n");
        sb.Append(".first{color:red}\n");
        for (var i = 0; i < pairs; i++)
        {
            sb.Append("/* section ").Append(i).Append(" */\n");
            sb.Append("/* a note about section ").Append(i).Append(" */\n");
            sb.Append(".rule-").Append(i).Append("{color:red}\n");
        }
        sb.Append(ending);
        return sb.ToString();
    }

    [Fact]
    public void Comments_That_Directly_Follow_Comments_Are_Matched_In_Linear_Time()
    {
        var env = new MockHostEnvironment();
        env.AddWebFile("a.css", CommentPairsAfterFirstRule(200));

        var sw = Stopwatch.StartNew();
        var actual = BuildBundle(env, files: ["a.css"]);
        sw.Stop();

        // the import pattern has a 5 second timeout, after which the file is
        // included without looking for imports, so the output alone can't tell
        Assert.True(sw.Elapsed < TimeSpan.FromSeconds(3),
            $"Bundling a CSS file with 200 comment pairs took {sw.Elapsed}, expected a linear match.");

        Assert.Contains(".first{color:red}", actual);
        Assert.Contains(".rule-0{color:red}", actual);
        Assert.Contains(".rule-199{color:red}", actual);
    }

    [Fact]
    public void Import_After_Rules_And_Many_Comment_Pairs_Is_Left_As_Is()
    {
        // browsers ignore @import after the first rule, so it is not processed
        var env = new MockHostEnvironment();
        env.AddWebFile("a.css", CommentPairsAfterFirstRule(200, "@import \"b.css\";\n.own{color:blue}"));
        env.AddWebFile("b.css", ".imported{color:green}");

        var sw = Stopwatch.StartNew();
        var actual = BuildBundle(env, files: ["a.css"]);
        sw.Stop();

        Assert.True(sw.Elapsed < TimeSpan.FromSeconds(3),
            $"Bundling a CSS file with 200 comment pairs took {sw.Elapsed}, expected a linear match.");
        Assert.Contains(".rule-199{color:red}", actual);
        Assert.Contains("@import", actual);
        Assert.DoesNotContain(".imported", actual);
    }

    [Fact]
    public void Import_After_Many_Leading_Comments_Is_Embedded()
    {
        var env = new MockHostEnvironment();

        var sb = new StringBuilder();
        for (var i = 0; i < 2000; i++)
            sb.Append("/* leading * comment ").Append(i).Append(" **/\n");
        sb.Append("@import \"b.css\";\n.own{color:red}");

        env.AddWebFile("a.css", sb.ToString());
        env.AddWebFile("b.css", ".imported{color:blue}");

        var sw = Stopwatch.StartNew();
        var actual = Normalize(BuildBundle(env, files: ["a.css"]));
        sw.Stop();

        Assert.True(sw.Elapsed < TimeSpan.FromSeconds(3),
            $"Bundling a CSS file with 2000 leading comments took {sw.Elapsed}, expected a linear match.");
        Assert.Equal(".imported{color:blue}.own{color:red}", actual);
    }

    [Fact]
    public void Unterminated_Comment_Before_Import_Leaves_The_File_As_Is()
    {
        var env = new MockHostEnvironment();
        env.AddWebFile("a.css", "/* header */\n/* not closed\n@import \"b.css\";\n.own{color:red}");
        env.AddWebFile("b.css", ".imported{color:blue}");

        var actual = BuildBundle(env, files: ["a.css"]);

        Assert.Contains("@import", actual);
        Assert.DoesNotContain(".imported", actual);
    }
}
