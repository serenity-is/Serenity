using System.Diagnostics;
using System.IO;

namespace Serenity.Web;

/// <summary>
/// Default <see cref="IStartedProcess"/> implementation that wraps a started <see cref="Process"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="StartedProcess"/> class.
/// </remarks>
/// <param name="process">The wrapped process.</param>
internal sealed class StartedProcess(Process process) : IStartedProcess
{
    private readonly Process process = process ?? throw new ArgumentNullException(nameof(process));

    /// <inheritdoc/>
    public StreamReader StandardOutput => process.StandardOutput;

    /// <inheritdoc/>
    public StreamReader StandardError => process.StandardError;

    /// <inheritdoc/>
    public TextWriter StandardInput => process.StandardInput;

    /// <inheritdoc/>
    public bool EnableRaisingEvents
    {
        get => process.EnableRaisingEvents;
        set => process.EnableRaisingEvents = value;
    }

    /// <inheritdoc/>
    public bool HasExited => process.HasExited;

    /// <inheritdoc/>
    public int ExitCode => process.ExitCode;

    /// <inheritdoc/>
    public bool WaitForExit(int milliseconds) => process.WaitForExit(milliseconds);

    /// <inheritdoc/>
    public void Kill(bool entireProcessTree) => process.Kill(entireProcessTree);
}
