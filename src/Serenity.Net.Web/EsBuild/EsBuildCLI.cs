using System.Diagnostics;

namespace Serenity.Web.EsBuild;

internal class EsBuildCLI(string path = null)
{
    private readonly string path = path ?? new EsBuildDownloader().Download();

    private string Run(string arguments = null, string stdin = null)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
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
            }
        };

        process.Start();

        if (!string.IsNullOrEmpty(stdin))
        {
            var input = process.StandardInput;
            input.Write(stdin);
            input.Flush();
            input.Close();
        }

        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0)
            throw new Exception(error);

        return output;
    }

    public string MinifyCssFile(string path, int lineLimit = 1000)
    {
        var arguments = $"--minify --loader=css {path} --line-limit={lineLimit}";
        return Run(arguments);
    }

    public string MinifyScriptFile(string path, int lineLimit = 1000)
    {
        var arguments = $"--minify --keep-names {path} --line-limit={lineLimit}";
        return Run(arguments);
    }

    public string MinifyCss(string code, int lineLimit = 1000)
    {
        var arguments = $"--minify --loader=css --line-limit={lineLimit}";
        return Run(arguments, code);
    }

    public string MinifyScript(string code, int lineLimit = 1000)
    {
        var arguments = $"--minify --keep-names --line-limit={lineLimit}";
        return Run(arguments, code);
    }
}