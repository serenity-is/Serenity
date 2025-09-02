using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Xml.Linq;
using HtmlHelper = Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper;

namespace Serenity.Demo.BasicSamples;

public static class BasicSamplesHelper
{
    public static string BasicSamplesModuleFile(this HtmlHelper helper, string module)
    {
        return UriHelper.Combine("~/Serenity.Demo.BasicSamples/esm/", GetRelativePathFor(helper, module));
    }

    public static HtmlString BasicSamplesSourceFile(this HtmlHelper helper, string file)
    {
        if (file == null || file.Length == 0)
            return null;

        var path = file.StartsWith('/') ? file[1..] : "demo.basicsamples" + GetRelativePathFor(helper, file);
        var href = $"https://github.com/serenity-is/Serenity/blob/{Uri.EscapeDataString(GetCommitId(helper))}/common-features/src/{path}";
        return new HtmlString($"<a target=\"blank\" style=\"font-weight: bold;\" href=\"{helper.Encode(href)}\">{helper.Encode(Path.GetFileName(file))}</a>");
    }

    private static string GetRelativePathFor(HtmlHelper helper, string file)
    {
        var module = (helper.ViewData?.Model as ModulePageModel)?.Module;
        string absolutePath;
        if (module != null && module.Contains("/esm/", StringComparison.OrdinalIgnoreCase))
        {
            var idx = module.IndexOf("/esm/", StringComparison.OrdinalIgnoreCase) + 4;
            absolutePath = Path.GetDirectoryName(module[idx..]).Replace('\\', '/') + '/';
        }
        else
        {
            var viewLocation = ((RazorView)helper.ViewContext.View).Path;
            absolutePath = Path.GetDirectoryName(viewLocation).Replace('\\', '/') + '/';
        }
        var relative = file.Replace('\\', '/');
        var question = relative.IndexOf('?', StringComparison.Ordinal);
        if (question >= 0)
        {
            relative = new Uri("x:" + absolutePath + relative[..question])
                .AbsolutePath[2..] + relative[question..];
        }
        else
            relative = new Uri("x:" + absolutePath + relative).AbsolutePath[2..];

        if (!file.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase))
            relative = relative.Replace("Areas/Serenity.Demo.BasicSamples", "Modules");

        return relative;
    }

    private static string cachedCommitId;

    private static string GetCommitId(HtmlHelper helper)
    {
        if (cachedCommitId != null)
            return cachedCommitId;

        var config = helper.ViewContext?.HttpContext?.RequestServices?.GetService<IConfiguration>();
        cachedCommitId = config?.GetValue<string>("SampleSettings:SerenityCommitId");
        if (cachedCommitId != null)
            return cachedCommitId;

        var packageId = typeof(BasicSamplesHelper).Assembly.GetName().Name.ToLowerInvariant();
        var version = typeof(BasicSamplesHelper).Assembly.GetName().Version;
        try
        {
            if (version == null)
                return cachedCommitId = "master";

            var nugetFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages");
            var versionStr = version.ToString();
            var nuspecFile = Path.Combine(nugetFolder, packageId, versionStr, packageId + ".nuspec");
            if (!File.Exists(nuspecFile))
            {
                if (version.Revision == 0 &&
                    versionStr.EndsWith(".0", StringComparison.Ordinal))
                {
                    versionStr = versionStr[..^2];
                    nuspecFile = Path.Combine(nugetFolder, packageId, versionStr, packageId + ".nuspec");
                    if (!File.Exists(nuspecFile))
                        return cachedCommitId = "master";
                }
                else
                    return cachedCommitId = "master";
            }

            var doc = XDocument.Load(nuspecFile);
            XNamespace ns = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd";
            var node = doc.Descendants(ns + "repository").FirstOrDefault();
            if (node == null)
                return null;
            var url = node.Attribute("url")?.Value;
            var commit = node.Attribute("commit")?.Value;
            if (!string.IsNullOrEmpty(url) &&
                !string.IsNullOrEmpty(commit) &&
                url.Equals("https://github.com/serenity-is/serenity.git", StringComparison.OrdinalIgnoreCase))
            {
                return cachedCommitId = commit;
            }
        }
        catch
        {
            return cachedCommitId = "master";
        }
        return cachedCommitId = "master";
    }

}