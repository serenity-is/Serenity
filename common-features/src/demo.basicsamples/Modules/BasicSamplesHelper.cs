using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using System.IO;
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

        string path = file.StartsWith('/') ?
            file[1..] :
            "Serenity.Demo.BasicSamples" + GetRelativePathFor(helper, file);

        return new HtmlString("<a target=\"blank\" style=\"font-weight: bold;\" href=\"" +
            helper.Encode("https://github.com/serenity-is/common-features/" +
                "blob/master/src/" + path) +
            "\">" + helper.Encode(Path.GetFileName(file)) + "</a>");
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
}