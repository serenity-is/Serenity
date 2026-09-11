using System.Diagnostics;
using System.IO;

namespace Serenity.Web.EsBuild;

internal interface IEsBuildCLI
{
    string MinifyCss(string code, int lineLimit = 1000);
    string MinifyScript(string code, int lineLimit = 1000);
}

internal class EsBuildCLI(string? path = null, Func<ProcessStartInfo, IStartedProcess>? processFactory = null) : IEsBuildCLI
{
    private readonly string path = path ?? new EsBuildDownloader().Download();
    private readonly Func<ProcessStartInfo, IStartedProcess>? processFactory = processFactory;

    private string Run(string arguments, string stdin)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = path,
            Arguments = arguments,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardInputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8,
            StandardOutputEncoding = Encoding.UTF8
        };

        var process = processFactory?.Invoke(startInfo) ??
            new StartedProcess(Process.Start(startInfo) ?? throw new InvalidOperationException("Could not start esbuild process!"));

        var input = process.StandardInput;
        input.Write(stdin ?? "");
        input.WriteLine();
        input.Flush();
        input.Close();

        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        if (!process.WaitForExit(5000))
            throw new OperationCanceledException("ESBuild CLI did not exit in 5 seconds");

        if (process.ExitCode != 0)
            throw new Exception($"Error {process.ExitCode}: {error}!");

        return output;
    }

    public string MinifyCss(string code, int lineLimit = 1000)
    {
        if (string.IsNullOrEmpty(code))
            return "";
        var arguments = $"--minify --loader=css --line-limit={lineLimit}";
        return Run(arguments, code);
    }

    public string MinifyScript(string code, int lineLimit = 1000)
    {
        if (string.IsNullOrEmpty(code))
            return "";
        var arguments = $"--minify --keep-names --line-limit={lineLimit}";
        return Run(arguments, code);
    }
}
