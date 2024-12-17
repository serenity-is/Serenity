using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Build;

public static partial class Shared
{
    public static readonly UTF8Encoding UTF8Bom = new(true);

    public static string DetectLineEnding(string content)
    {
        if (string.IsNullOrEmpty(content))
            return null;

        int lfCount = 0;
        int crlfCount = 0;
        int startIndex = 0;
        while (startIndex < content.Length)
        {
            var idx = content.IndexOf('\n', startIndex);
            if (idx < 0)
                break;

            if (idx > 0 && content[idx - 1] == '\r')
                crlfCount++;
            else
                lfCount++;

            startIndex = idx + 1;
        }

        if (lfCount > crlfCount)
            return "\n";
        else if (crlfCount > lfCount)
            return "\r\n";

        return null;
    }

    public static void CleanDirectory(string path, bool ensure = false)
    {
#if IsFeatureBuild || IsTemplateBuild
        path = Path.Combine(Root, path);
#endif
        if (!Directory.Exists(path))
        {
            if (ensure)
                Directory.CreateDirectory(path);
            return;
        }

        foreach (var file in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
        {
            try
            {
                File.Delete(file);
            }
            catch
            {
            }
        }

        foreach (var dir in Directory.GetDirectories(path, "*.*", SearchOption.AllDirectories))
        {
            try
            {
                Directory.Delete(dir);
            }
            catch
            {
            }
        }
    }

    public static int StartProcess(string name, string arguments, string workingDirectory)
    {
        var info = new ProcessStartInfo(name)
        {
            WorkingDirectory = workingDirectory
        };
        if (arguments != null)
            info.Arguments = arguments;

        var process = Process.Start(info);
        process.WaitForExit();
        var exitCode = process.ExitCode;
        return exitCode;
    }

    public static void ExitWithError(string error, int errorCode = 1)
    {
        Console.WriteLine(error);
        Environment.Exit(errorCode);
    }
}