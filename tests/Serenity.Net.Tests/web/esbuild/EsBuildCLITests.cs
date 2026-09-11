using System.Diagnostics;

namespace Serenity.Web.EsBuild;

public class EsBuildCLITests
{
    private sealed class FakeStartedProcess : IStartedProcess
    {
        public System.IO.StringWriter Input { get; } = new();
        public string OutputText { get; set; } = "";
        public string ErrorText { get; set; } = "";
        public bool WaitResult { get; set; } = true;
        public int ExitCode { get; set; }
        public System.IO.StreamReader StandardOutput => new(new System.IO.MemoryStream(Encoding.UTF8.GetBytes(OutputText)));
        public System.IO.StreamReader StandardError => new(new System.IO.MemoryStream(Encoding.UTF8.GetBytes(ErrorText)));
        public System.IO.TextWriter StandardInput => Input;
        public bool EnableRaisingEvents { get; set; }
        public bool HasExited { get; set; }
        public bool WaitForExit(int milliseconds) => WaitResult;
        public void Kill(bool entireProcessTree)
        {
        }
    }

    [Fact]
    public void MinifyCss_Returns_Empty_For_Empty_Input()
    {
        Assert.Equal("", new EsBuildCLI("path").MinifyCss(""));
        Assert.Equal("", new EsBuildCLI("path").MinifyCss(null!));
    }

    [Fact]
    public void MinifyScript_Returns_Empty_For_Empty_Input()
    {
        Assert.Equal("", new EsBuildCLI("path").MinifyScript(""));
        Assert.Equal("", new EsBuildCLI("path").MinifyScript(null!));
    }

    [Fact]
    public void MinifyCss_Runs_CLI_With_Expected_Arguments()
    {
        var fake = new FakeStartedProcess { OutputText = "MINIFIED" };
        ProcessStartInfo? captured = null;
        var cli = new EsBuildCLI("esbuild", startInfo =>
        {
            captured = startInfo;
            return fake;
        });

        var result = cli.MinifyCss("body{}", 55);

        Assert.Equal("MINIFIED", result);
        Assert.Equal("esbuild", captured!.FileName);
        Assert.Contains("--minify", captured.Arguments);
        Assert.Contains("--loader=css", captured.Arguments);
        Assert.Contains("--line-limit=55", captured.Arguments);
        Assert.Contains("body{}", fake.Input.ToString());
    }

    [Fact]
    public void MinifyScript_Runs_CLI_With_Expected_Arguments()
    {
        var fake = new FakeStartedProcess { OutputText = "MINJS" };
        ProcessStartInfo? captured = null;
        var cli = new EsBuildCLI("esbuild", startInfo =>
        {
            captured = startInfo;
            return fake;
        });

        var result = cli.MinifyScript("var a = 1;");

        Assert.Equal("MINJS", result);
        Assert.Contains("--keep-names", captured!.Arguments);
    }

    [Fact]
    public void Run_Throws_On_Timeout()
    {
        var fake = new FakeStartedProcess { WaitResult = false };
        var cli = new EsBuildCLI("esbuild", _ => fake);

        Assert.Throws<OperationCanceledException>(() => cli.MinifyScript("var a = 1;"));
    }

    [Fact]
    public void Run_Throws_On_Non_Zero_ExitCode()
    {
        var fake = new FakeStartedProcess { ExitCode = 2, ErrorText = "bad" };
        var cli = new EsBuildCLI("esbuild", _ => fake);

        var exception = Assert.Throws<Exception>(() => cli.MinifyCss("body{}"));
        Assert.Contains("Error 2: bad", exception.Message);
    }
}
