using System.Diagnostics;

namespace Serenity.Web;

public class ChildProcessJobTests
{
    [Fact]
    public void Create_Returns_Null_Only_When_Not_On_Windows()
    {
        using var job = ChildProcessJob.Create();

        Assert.Equal(OperatingSystem.IsWindows(), job != null);
    }

    [Fact]
    public void Closing_The_Job_Kills_Assigned_Process_And_Its_Descendants()
    {
        Assert.SkipUnless(OperatingSystem.IsWindows(), "Job objects are only available on Windows");

        var job = ChildProcessJob.Create()!;
        var marker = "childprocessjob" + Guid.NewGuid().ToString("N");

        // cmd stays alive as long as the nested cmd it started, which is the descendant here
        using var process = Process.Start(new ProcessStartInfo("cmd",
            $"/c cmd /c \"ping -n 60 127.0.0.1 >nul & rem {marker}\"")
        {
            CreateNoWindow = true,
            UseShellExecute = false
        })!;

        try
        {
            Assert.True(job.TryAssign(process));
            Assert.False(process.WaitForExit(500));

            job.Dispose();

            Assert.True(process.WaitForExit(10000));
        }
        finally
        {
            job.Dispose();
            if (!process.HasExited)
                process.Kill(entireProcessTree: true);
        }
    }

    [Fact]
    public void Shared_Returns_The_Same_Instance()
    {
        Assert.Same(ChildProcessJob.Shared, ChildProcessJob.Shared);
    }
}
