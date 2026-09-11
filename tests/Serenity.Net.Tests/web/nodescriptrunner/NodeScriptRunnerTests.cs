using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Serenity.Web;

public class NodeScriptRunnerTests
{
    private sealed class FakeStartedProcess : IStartedProcess
    {
        public System.IO.StreamReader StandardOutput { get; set; } =
            new(System.IO.Stream.Null);
        public System.IO.StreamReader StandardError { get; set; } =
            new(System.IO.Stream.Null);
        public System.IO.TextWriter StandardInput { get; set; } = System.IO.TextWriter.Null;
        public bool EnableRaisingEvents { get; set; }
        public bool HasExited { get; set; }
        public int ExitCode { get; set; }
        public bool WaitForExitResult { get; set; } = true;
        public bool Killed { get; private set; }
        public bool EntireTreeKilled { get; private set; }

        public bool WaitForExit(int milliseconds) => WaitForExitResult;

        public void Kill(bool entireProcessTree)
        {
            Killed = true;
            EntireTreeKilled = entireProcessTree;
            HasExited = true;
        }
    }

    [Fact]
    public void Constructor_Throws_For_Empty_Arguments()
    {
        Assert.Throws<ArgumentException>(() =>
            new NodeScriptRunner("script", workingDirectory: ""));
        Assert.Throws<ArgumentException>(() =>
            new NodeScriptRunner("", workingDirectory: "dir"));
        Assert.Throws<ArgumentException>(() =>
            new NodeScriptRunner("script", workingDirectory: "dir", pkgManagerCommand: ""));
    }

    [Fact]
    public void Constructor_Builds_Node_Arguments()
    {
        ProcessStartInfo? captured = null;
        _ = new NodeScriptRunner("myscript", "arg1 arg2", "workdir", processFactory: startInfo =>
        {
            captured = startInfo;
            return new FakeStartedProcess();
        });

        Assert.Equal("node", captured!.FileName);
        Assert.Equal("--run myscript -- arg1 arg2", captured.Arguments);
        Assert.Equal("workdir", captured.WorkingDirectory);
        Assert.False(captured.UseShellExecute);
        Assert.True(captured.RedirectStandardOutput);
        Assert.True(captured.RedirectStandardError);
    }

    [Fact]
    public void Constructor_Builds_Npm_Arguments()
    {
        ProcessStartInfo? captured = null;
        _ = new NodeScriptRunner("myscript", null, "workdir", pkgManagerCommand: "npm",
            processFactory: startInfo =>
            {
                captured = startInfo;
                return new FakeStartedProcess();
            });

        if (OperatingSystem.IsWindows())
        {
            Assert.Equal("cmd", captured!.FileName);
            Assert.Equal("/c npm run myscript -- ", captured.Arguments);
        }
        else
        {
            Assert.Equal("npm", captured!.FileName);
            Assert.Equal("run myscript -- ", captured.Arguments);
        }
    }

    [Fact]
    public void Constructor_Applies_Environment_Variables()
    {
        ProcessStartInfo? captured = null;
        _ = new NodeScriptRunner("myscript", null, "workdir",
            new Dictionary<string, string> { ["MY_VAR"] = "MY_VALUE" },
            processFactory: startInfo =>
            {
                captured = startInfo;
                return new FakeStartedProcess();
            });

        Assert.Equal("MY_VALUE", captured!.Environment["MY_VAR"]);
    }

    [Fact]
    public void Constructor_Wraps_Process_Factory_Exceptions()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            new NodeScriptRunner("myscript", null, "workdir",
                processFactory: _ => throw new InvalidOperationException("boom")));

        Assert.Contains("Failed to start 'node'", exception.Message);
    }

    [Fact]
    public void Constructor_Writes_Diagnostic_Event_When_Enabled()
    {
        var listener = new DiagnosticListener("Test");
        var events = new List<string>();
        using var subscription = listener.Subscribe(new ListenerObserver(name => events.Add(name)));
        var fake = new FakeStartedProcess();

        _ = new NodeScriptRunner("script", null, "dir", diagnosticSource: listener,
            processFactory: _ => fake);

        Assert.Contains("Microsoft.AspNetCore.NodeServices.Npm.NpmStarted", events);
    }

    private sealed class ListenerObserver(Action<string> onNext) : IObserver<KeyValuePair<string, object?>>
    {
        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(KeyValuePair<string, object?> value) => onNext(value.Key);
    }

    [Fact]
    public void Dispose_Kills_Running_Process()
    {
        var fake = new FakeStartedProcess { HasExited = false };
        var runner = new NodeScriptRunner("script", "args", "dir", processFactory: _ => fake);

        ((IDisposable)runner).Dispose();

        Assert.True(fake.Killed);
        Assert.True(fake.EntireTreeKilled);
    }

    [Fact]
    public void Dispose_Does_Nothing_When_Process_Already_Exited()
    {
        var fake = new FakeStartedProcess { HasExited = true };
        var runner = new NodeScriptRunner("script", "args", "dir", processFactory: _ => fake);

        ((IDisposable)runner).Dispose();

        Assert.False(fake.Killed);
    }

    [Fact]
    public async Task AttachToLogger_Logs_Output_With_Levels_And_Strips_Colors()
    {
        var stdOutStream = new BlockingStream();
        var stdErrStream = new BlockingStream();
        var fake = new FakeStartedProcess
        {
            StandardOutput = new System.IO.StreamReader(stdOutStream),
            StandardError = new System.IO.StreamReader(stdErrStream)
        };
        var runner = new NodeScriptRunner("script", "args", "dir", processFactory: _ => fake);
        var logger = new RecordingLogger();

        runner.AttachToLogger(logger);

        stdOutStream.Write("info line\n\x001b[31mred line\x001b[0m\n");
        stdOutStream.Complete();
        stdErrStream.Write("Error: bad\nWarning: warn\nWarn: warn2\nplain\n");
        stdErrStream.Complete();

        var deadline = DateTime.UtcNow.AddSeconds(20);
        while (logger.Entries.Count < 6 && DateTime.UtcNow < deadline)
            await Task.Delay(25, TestContext.Current.CancellationToken);

        Assert.Contains(logger.Entries, e => e.Level == LogLevel.Information && e.Message.Trim() == "info line");
        Assert.Contains(logger.Entries, e => e.Message.Trim() == "red line");
        Assert.Contains(logger.Entries, e => e.Level == LogLevel.Error && e.Message.Trim() == "Error: bad");
        Assert.Contains(logger.Entries, e => e.Level == LogLevel.Warning && e.Message.Trim() == "Warning: warn");
        Assert.Contains(logger.Entries, e => e.Level == LogLevel.Warning && e.Message.Trim() == "Warn: warn2");
        Assert.Contains(logger.Entries, e => e.Level == LogLevel.Information && e.Message.Trim() == "plain");
    }
}
