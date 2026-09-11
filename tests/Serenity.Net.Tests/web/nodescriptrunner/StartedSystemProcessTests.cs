using System.Diagnostics;

namespace Serenity.Web;

public class StartedSystemProcessTests
{
    [Fact]
    public void Wraps_A_Started_Process()
    {
        var startInfo = OperatingSystem.IsWindows()
            ? new ProcessStartInfo("cmd", "/c exit 0")
            : new ProcessStartInfo("sh", "-c \"exit 0\"");
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
        startInfo.RedirectStandardInput = true;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;

        var process = Process.Start(startInfo)!;
        var started = new StartedProcess(process);
        started.EnableRaisingEvents = true;

        Assert.NotNull(started.StandardOutput);
        Assert.NotNull(started.StandardError);
        Assert.NotNull(started.StandardInput);
        Assert.True(started.WaitForExit(15000));
        Assert.True(started.HasExited);
        Assert.Equal(0, started.ExitCode);
    }
}
