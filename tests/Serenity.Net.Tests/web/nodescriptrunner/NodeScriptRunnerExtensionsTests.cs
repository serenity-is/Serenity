using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Threading;

namespace Serenity.Extensions.DependencyInjection;

public class NodeScriptRunnerExtensionsTests
{
    private sealed class FakeHostLifetime : IHostApplicationLifetime
    {
        public CancellationToken ApplicationStarted => default;
        public CancellationToken ApplicationStopping => default;
        public CancellationToken ApplicationStopped => default;
        public void StopApplication()
        {
        }
    }

    private sealed class FakeStartedProcess : IStartedProcess
    {
        public System.IO.StreamReader StandardOutput { get; set; } = new(System.IO.Stream.Null);
        public System.IO.StreamReader StandardError { get; set; } = new(System.IO.Stream.Null);
        public System.IO.TextWriter StandardInput { get; set; } = System.IO.TextWriter.Null;
        public bool EnableRaisingEvents { get; set; }
        public bool HasExited { get; set; }
        public int ExitCode { get; set; }
        public bool WaitForExit(int milliseconds) => true;
        public void Kill(bool entireProcessTree)
        {
        }
    }

    private static ServiceProvider CreateServices(string? startNodeScripts)
    {
        var config = new Dictionary<string, string?>();
        if (startNodeScripts != null)
            config["StartNodeScripts"] = startNodeScripts;

        return new ServiceCollection()
            .AddSingleton<IHostApplicationLifetime>(new FakeHostLifetime())
            .AddSingleton<DiagnosticSource>(new DiagnosticListener("Test"))
            .AddSingleton<IWebHostEnvironment>(new MockHostEnvironment())
            .AddSingleton<IConfiguration>(new ConfigurationBuilder()
                .AddInMemoryCollection(config).Build())
            .BuildServiceProvider();
    }

    [Fact]
    public void UseNodeScriptRunner_Starts_Configured_Scripts()
    {
        var captured = new List<ProcessStartInfo>();
        var app = new ApplicationBuilder(CreateServices("build;watch --port 3000"));

        app.UseNodeScriptRunner(processFactory: startInfo =>
        {
            captured.Add(startInfo);
            return new FakeStartedProcess();
        });

        Assert.Equal(2, captured.Count);
        Assert.Equal("--run build -- ", captured[0].Arguments);
        Assert.Contains("--run watch -- --port 3000", captured[1].Arguments);
        Assert.False(string.IsNullOrEmpty(captured[0].WorkingDirectory));
    }

    [Fact]
    public void UseNodeScriptRunner_Does_Nothing_When_Not_Configured()
    {
        var captured = new List<ProcessStartInfo>();
        var app = new ApplicationBuilder(CreateServices(null));

        app.UseNodeScriptRunner(processFactory: startInfo =>
        {
            captured.Add(startInfo);
            return new FakeStartedProcess();
        });

        Assert.Empty(captured);
    }

    [Fact]
    public void UseNodeScriptRunner_Does_Nothing_When_Empty()
    {
        var captured = new List<ProcessStartInfo>();
        var app = new ApplicationBuilder(CreateServices(""));

        app.UseNodeScriptRunner(processFactory: startInfo =>
        {
            captured.Add(startInfo);
            return new FakeStartedProcess();
        });

        Assert.Empty(captured);
    }

    [Fact]
    public void StartNodeScript_Starts_Process()
    {
        ProcessStartInfo? captured = null;
        var app = new ApplicationBuilder(CreateServices(null));

        app.StartNodeScript("build", "arg", processFactory: startInfo =>
        {
            captured = startInfo;
            return new FakeStartedProcess();
        });

        Assert.NotNull(captured);
        Assert.Equal("--run build -- arg", captured!.Arguments);
    }

    [Fact]
    public void StartNodeScript_Uses_Provided_WorkingDirectory()
    {
        ProcessStartInfo? captured = null;
        var app = new ApplicationBuilder(CreateServices(null));

        app.StartNodeScript("build", workingDirectory: "/custom", processFactory: startInfo =>
        {
            captured = startInfo;
            return new FakeStartedProcess();
        });

        Assert.Equal("/custom", captured!.WorkingDirectory);
    }
}
