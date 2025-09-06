using System.Diagnostics;

namespace Serenity.CodeGenerator;

public interface IProcessExecutor
{
    bool StartAndWaitForExit(Process process, int timeout, out string output, out string errorOutput);
}