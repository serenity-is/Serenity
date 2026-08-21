using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serenity.Web;
using Serenity.Web.SpaServices;
using System.Diagnostics;

namespace Serenity.Extensions.DependencyInjection;

/// <summary>
/// Contains extension methods for <see cref="NodeScriptRunner"/>.
/// </summary>
public static class NodeScriptRunnerExtensions
{
    private const string LogCategoryName = "Serenity.Web.NodeScriptRunner";

    /// <summary>
    /// Starts a node (NPM) script and attaches its output to the application logger.
    /// </summary>
    /// <param name="appBuilder">The application builder.</param>
    /// <param name="scriptName">The script name in <c>package.json</c>.</param>
    /// <param name="arguments">The arguments to pass to the script.</param>
    /// <param name="workingDirectory">The working directory; defaults to the content root path.</param>
    /// <param name="envVars">Optional environment variables to set for the process.</param>
    /// <param name="pkgManagerCommand">The package manager command (defaults to <c>npm</c>).</param>
    public static void StartNodeScript(this IApplicationBuilder appBuilder, string scriptName, 
        string arguments = null, string workingDirectory = null, 
        IDictionary<string, string> envVars = null, string pkgManagerCommand = "npm")
    {
        var applicationStoppingToken = appBuilder.ApplicationServices
            .GetRequiredService<IHostApplicationLifetime>().ApplicationStopping;
        var logger = LoggerFinder.GetOrCreateLogger(appBuilder, LogCategoryName);
        var diagnosticSource = appBuilder.ApplicationServices.GetRequiredService<DiagnosticSource>();
        workingDirectory ??= appBuilder.ApplicationServices.GetRequiredService<IWebHostEnvironment>().ContentRootPath;

        new NodeScriptRunner(scriptName, arguments, workingDirectory, envVars, pkgManagerCommand,
            diagnosticSource, applicationStoppingToken)
                .AttachToLogger(logger);
    }
}