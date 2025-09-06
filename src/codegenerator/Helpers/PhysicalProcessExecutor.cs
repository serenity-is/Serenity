using System.Diagnostics;

namespace Serenity.CodeGenerator;

public class PhysicalProcessExecutor : IProcessExecutor
{
    public bool StartAndWaitForExit(Process process, int timeout, out string output, out string errorOutput)
    {
        try
        {
            process.Start();
            output = null;
            errorOutput = null;
            if (process.StartInfo.RedirectStandardOutput)
                output = process.StandardOutput.ReadToEnd();
            if (process.StartInfo.RedirectStandardError)
                errorOutput = process.StandardError.ReadToEnd();
            return process.WaitForExit(10000);
        }
        catch
        {
            output = null;
            errorOutput = null;
            return false;
        }
    }
}