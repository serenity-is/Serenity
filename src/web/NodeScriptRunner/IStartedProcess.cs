using System.IO;

namespace Serenity.Web;

/// <summary>
/// Abstraction for a started system process, so it can be replaced in tests.
/// </summary>
public interface IStartedProcess
{
    /// <summary>Gets the standard output reader.</summary>
    StreamReader StandardOutput { get; }

    /// <summary>Gets the standard error reader.</summary>
    StreamReader StandardError { get; }

    /// <summary>Gets the standard input writer.</summary>
    TextWriter StandardInput { get; }

    /// <summary>Gets or sets a value indicating whether the process raises events.</summary>
    bool EnableRaisingEvents { get; set; }

    /// <summary>Gets whether the process has exited.</summary>
    bool HasExited { get; }

    /// <summary>Gets the process exit code.</summary>
    int ExitCode { get; }

    /// <summary>Waits for the process to exit.</summary>
    /// <param name="milliseconds">The timeout in milliseconds.</param>
    /// <returns>True if the process exited within the timeout.</returns>
    bool WaitForExit(int milliseconds);

    /// <summary>Kills the process.</summary>
    /// <param name="entireProcessTree">True to kill the entire process tree.</param>
    void Kill(bool entireProcessTree);
}
