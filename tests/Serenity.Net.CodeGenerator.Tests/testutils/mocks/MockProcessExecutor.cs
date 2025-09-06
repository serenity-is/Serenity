using Serenity.CodeGenerator;
using System.Diagnostics;

namespace Serenity.TestUtils;

public class MockProcessExecutor : IProcessExecutor
{
    public List<(Process, int)> Calls = new();

    public bool StartAndWaitForExit(Process process, int timeout, out string output, out string errorOutput)
    {
        Calls.Add((process, timeout));
        if (_handler != null)
        {
            (var result, output, errorOutput) = _handler(process, timeout);
            return result;
        }

        (output, errorOutput) = ("", "");
        return true;
    }

    private Func<Process, int, (bool, string, string)> _handler;

    public void OnStart(Func<Process, int, (bool, string, string)> handler)
    {
        _handler = handler;
    }
}