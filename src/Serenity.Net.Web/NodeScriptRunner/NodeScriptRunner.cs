// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;
using Serenity.Web.SpaServices;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Serenity.Web;

/// <summary>
/// Executes the <c>script</c> entries defined in a <c>package.json</c> file,
/// capturing any output written to stdio.
/// </summary>
public sealed class NodeScriptRunner : IDisposable
{
    private Process npmProcess;
    private EventedStreamReader StdOut { get; }
    private EventedStreamReader StdErr { get; }

    private static readonly Regex AnsiColorRegex = new("\x001b\\[[0-9;]*m", RegexOptions.None, TimeSpan.FromSeconds(1));

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="scriptName">Script name</param>
    /// <param name="arguments">Arguments</param>
    /// <param name="workingDirectory">Working directory</param>
    /// <param name="envVars">Environment variables</param>
    /// <param name="pkgManagerCommand">Package manager command. Defaul is "npm"</param>
    /// <param name="diagnosticSource">Diagnostics source</param>
    /// <param name="applicationStoppingToken">Application stopping token</param>
    /// <exception cref="ArgumentException">One of arguments is null</exception>
    public NodeScriptRunner(string scriptName, 
        string arguments = null, string workingDirectory = null,
        IDictionary<string, string> envVars = null, string pkgManagerCommand = "npm", 
        DiagnosticSource diagnosticSource = null, CancellationToken applicationStoppingToken = default)
    {
        if (string.IsNullOrEmpty(workingDirectory))
        {
            throw new ArgumentException("Cannot be null or empty.", nameof(workingDirectory));
        }

        if (string.IsNullOrEmpty(scriptName))
        {
            throw new ArgumentException("Cannot be null or empty.", nameof(scriptName));
        }

        if (string.IsNullOrEmpty(pkgManagerCommand))
        {
            throw new ArgumentException("Cannot be null or empty.", nameof(pkgManagerCommand));
        }

        var exeToRun = pkgManagerCommand;
        var completeArguments = $"run {scriptName} -- {arguments ?? string.Empty}";
        if (OperatingSystem.IsWindows())
        {
            // On Windows, the node executable is a .cmd file, so it can't be executed
            // directly (except with UseShellExecute=true, but that's no good, because
            // it prevents capturing stdio). So we need to invoke it via "cmd /c".
            exeToRun = "cmd";
            completeArguments = $"/c {pkgManagerCommand} {completeArguments}";
        }

        var processStartInfo = new ProcessStartInfo(exeToRun)
        {
            Arguments = completeArguments,
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = workingDirectory
        };

        if (envVars != null)
        {
            foreach (var keyValuePair in envVars)
            {
                processStartInfo.Environment[keyValuePair.Key] = keyValuePair.Value;
            }
        }

        npmProcess = LaunchNodeProcess(processStartInfo, pkgManagerCommand);
        StdOut = new EventedStreamReader(npmProcess.StandardOutput);
        StdErr = new EventedStreamReader(npmProcess.StandardError);

        applicationStoppingToken.Register(((IDisposable)this).Dispose);

        if (diagnosticSource.IsEnabled("Microsoft.AspNetCore.NodeServices.Npm.NpmStarted"))
        {
            WriteDiagnosticEvent(
                diagnosticSource,
                "Microsoft.AspNetCore.NodeServices.Npm.NpmStarted",
                new
                {
                    processStartInfo,
                    process = npmProcess
                });
        }

        [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026",
            Justification = "The values being passed into Write have the commonly used properties being preserved with DynamicDependency.")]
        static void WriteDiagnosticEvent<TValue>(DiagnosticSource diagnosticSource, string name, TValue value)
            => diagnosticSource.Write(name, value);
    }

    /// <summary>
    /// Attaches to the logger
    /// </summary>
    /// <param name="logger">Logger</param>
    public void AttachToLogger(ILogger logger)
    {
#pragma warning disable CA2254 // Template should be a static expression
        // When the node task emits complete lines, pass them through to the real logger
        StdOut.OnReceivedLine += line =>
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                // Node tasks commonly emit ANSI colors, but it wouldn't make sense to forward
                // those to loggers (because a logger isn't necessarily any kind of terminal)
                logger.LogInformation(StripAnsiColors(line));
            }
        };

        StdErr.OnReceivedLine += line =>
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                // workaround for esbuild as it logs to stderr
                if (line.StartsWith("[watch]"))
                    logger.LogInformation(StripAnsiColors(line));
                else
                    logger.LogError(StripAnsiColors(line));
            }
        };
#pragma warning restore CA2254 // Template should be a static expression

        /*
        // But when it emits incomplete lines, assume this is progress information and
        // hence just pass it through to StdOut regardless of logger config.
        StdErr.OnReceivedChunk += chunk =>
        {
            Debug.Assert(chunk.Array != null);

            var containsNewline = Array.IndexOf(
                chunk.Array, '\n', chunk.Offset, chunk.Count) >= 0;
            if (!containsNewline)
            {
                Console.Write(chunk.Array, chunk.Offset, chunk.Count);
            }
        };*/
    }

    private static string StripAnsiColors(string line)
        => AnsiColorRegex.Replace(line, string.Empty);

    private static Process LaunchNodeProcess(ProcessStartInfo startInfo, string commandName)
    {
        try
        {
            var process = Process.Start(startInfo)!;

            // See equivalent comment in OutOfProcessNodeInstance.cs for why
            process.EnableRaisingEvents = true;

            return process;
        }
        catch (Exception ex)
        {
            var message = $"Failed to start '{commandName}'. To resolve this:.\n\n"
                        + $"[1] Ensure that '{commandName}' is installed and can be found in one of the PATH directories.\n"
                        + $"    Current PATH enviroment variable is: {Environment.GetEnvironmentVariable("PATH")}\n"
                        + "    Make sure the executable is in one of those directories, or update your PATH.\n\n"
                        + "[2] See the InnerException for further details of the cause.";
            throw new InvalidOperationException(message, ex);
        }
    }

    void IDisposable.Dispose()
    {
        if (npmProcess != null && !npmProcess.HasExited)
        {
            npmProcess.Kill(entireProcessTree: true);
            npmProcess = null;
        }
    }
}