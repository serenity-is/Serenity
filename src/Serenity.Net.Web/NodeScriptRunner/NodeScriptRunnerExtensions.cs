using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serenity.Web;
using Serenity.Web.SpaServices;
using System.Diagnostics;

namespace Serenity.Extensions.DependencyInjection;

public static class NodeScriptRunnerExtensions
{
    private const string LogCategoryName = "Serenity.Web.NodeScriptRunner";

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