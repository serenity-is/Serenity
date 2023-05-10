using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serenity.Web;
using Serenity.Web.SpaServices;
using System.Diagnostics;

namespace Serenity.Extensions.DependencyInjection;

/// <summary>
/// Contains extensions for <see cref="NodeScriptRunner"/>
/// </summary>
public static class NodeScriptRunnerExtensions
{
    private const string LogCategoryName = "Serenity.Web.NodeScriptRunner";

    /// <summary>
    /// Starts a node (NPM) script
    /// </summary>
    /// <param name="appBuilder">Application builder</param>
    /// <param name="scriptName">Script name in package.json</param>
    /// <param name="arguments">Arguments</param>
    /// <param name="workingDirectory">Working directory</param>
    /// <param name="envVars">Environment variables</param>
    /// <param name="pkgManagerCommand">Package manager command (default is "npm")</param>
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