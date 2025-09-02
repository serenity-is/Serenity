using Microsoft.Extensions.Logging;
using System.IO;

namespace Serenity.Web.EsBuild;

internal class EsBuildMinifier(ILogger<EsBuildMinifier> logger = null) : ICssMinifier, IScriptMinifier
{
    private EsBuildCLI cli;

    private EsBuildCLI GetCLI()
    {
        if (cli != null)
            return cli;

        var downloader = new EsBuildDownloader();
        var targetDirectory = Path.Combine(Path.GetTempPath(), ".esbuild");
        var executablePath = downloader.Download(targetDirectory: targetDirectory);
        return (cli = new EsBuildCLI(executablePath));
    }

    public CssMinifyResult MinifyCss(string source, CssMinifyOptions options)
    {
        try
        {
            return new CssMinifyResult
            {
                Code = GetCLI().MinifyCss(source, options.LineBreakThreshold == 0 ?
                    int.MaxValue - 1000 : options.LineBreakThreshold)
            };
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error minifying CSS");
            return new CssMinifyResult { Code = source, HasErrors = true };
        }
    }

    public ScriptMinifyResult MinifyScript(string source, ScriptMinifyOptions options)
    {
        try
        {
            return new ScriptMinifyResult
            {
                Code = GetCLI().MinifyScript(source, options.LineBreakThreshold == 0 ?
                    int.MaxValue - 1000 : options.LineBreakThreshold)
            };
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error minifying script");
            return new ScriptMinifyResult { Code = source, HasErrors = true };
        }
    }
}