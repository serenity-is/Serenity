using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serenity.Abstractions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;

namespace Serenity.Web
{
    public static class HtmlScriptExtensions
    {
        public static HtmlString Stylesheet(this IHtmlHelper helper, string cssUrl)
        {
            if (helper == null)
                throw new ArgumentNullException(nameof(helper));

            if (cssUrl == null)
                throw new ArgumentNullException(nameof(cssUrl));

            var context = helper.ViewContext.HttpContext;
            var css = context.RequestServices.GetRequiredService<ICssBundleManager>()
                .GetCssBundle(cssUrl);
            var included = GetIncludedCssList(context);

            if (!included.Contains(css))
            {
                included.Add(css);

                return new HtmlString(string.Format(CultureInfo.InvariantCulture,
                    "    <link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\"/>\n",
                    WebUtility.HtmlEncode(context.RequestServices.GetRequiredService<IContentHashCache>()
                        .ResolveWithHash(context.Request.PathBase, css))));
            }
            else
                return HtmlString.Empty;
        }

        public static HtmlString StyleBundle(this IHtmlHelper helper, string bundleKey)
        {
            if (helper == null)
                throw new ArgumentNullException(nameof(helper));

            if (string.IsNullOrEmpty(bundleKey))
                throw new ArgumentNullException(nameof(bundleKey));

            var context = helper.ViewContext.HttpContext;
            var bundleManager = context.RequestServices.GetRequiredService<ICssBundleManager>();
            var scriptManager = context.RequestServices.GetRequiredService<IDynamicScriptManager>();
            var hostEnvironment = context.RequestServices.GetRequiredService<IWebHostEnvironment>();
            var contentHashCache = context.RequestServices.GetRequiredService<IContentHashCache>();

            if (bundleManager.IsEnabled)
                return Stylesheet(helper, "dynamic://CssBundle." + bundleKey);

            StringBuilder sb = new StringBuilder();
            foreach (var include in bundleManager.GetBundleIncludes(bundleKey))
            {
                var cssUrl = include;
                if (string.IsNullOrEmpty(cssUrl))
                    continue;

                if (cssUrl.StartsWith("dynamic://", StringComparison.OrdinalIgnoreCase))
                {
                    var scriptName = cssUrl[10..];
                    cssUrl = scriptManager.GetScriptInclude(scriptName, ".css");
                    cssUrl = VirtualPathUtility.ToAbsolute(context, "~/DynJS.axd/" + cssUrl);
                }
                else
                {
                    cssUrl = BundleUtils.ExpandVersionVariable(hostEnvironment.WebRootPath, cssUrl);
                    cssUrl = VirtualPathUtility.ToAbsolute(context, cssUrl);
                }

                var cssList = GetIncludedCssList(context);

                if (!cssList.Contains(cssUrl))
                {
                    cssList.Add(cssUrl);
                    sb.AppendLine(string.Format(CultureInfo.InvariantCulture,
                        "    <link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\"/>\n",
                        WebUtility.HtmlEncode(contentHashCache.ResolveWithHash(context.Request.PathBase, cssUrl))));
                }
            }

            return new HtmlString(sb.ToString());
        }

        public static HtmlString Script(this IHtmlHelper helper, string includeJS)
        {
            if (helper == null)
                throw new ArgumentNullException(nameof(helper));

            if (string.IsNullOrEmpty(includeJS))
                throw new ArgumentNullException(nameof(includeJS));

            var context = helper.ViewContext.HttpContext;
            var script = context.RequestServices.GetRequiredService<IScriptBundleManager>()
                .GetScriptBundle(includeJS);
            var scripts = GetIncludedScripts(context);

            if (!scripts.Contains(script))
            {
                scripts.Add(script);

                return new HtmlString(string.Format(CultureInfo.InvariantCulture,
                    "    <script src=\"{0}\" type=\"text/javascript\"></script>\n",
                    WebUtility.HtmlEncode(context.RequestServices.GetRequiredService<IContentHashCache>()
                        .ResolveWithHash(context.Request.PathBase, script))));
            }
            else
                return new HtmlString("");
        }

        public static HtmlString ScriptBundle(this IHtmlHelper helper, string bundleKey)
        {
            if (helper == null)
                throw new ArgumentNullException(nameof(helper));

            if (string.IsNullOrEmpty(bundleKey))
                throw new ArgumentNullException(nameof(bundleKey));

            var context = helper.ViewContext.HttpContext;
            var bundleManager = context.RequestServices.GetRequiredService<IScriptBundleManager>();
            var scriptManager = context.RequestServices.GetRequiredService<IDynamicScriptManager>();
            var hostEnvironment = context.RequestServices.GetRequiredService<IWebHostEnvironment>();
            var contentHashCache = context.RequestServices.GetRequiredService<IContentHashCache>();

            if (bundleManager.IsEnabled)
                return Script(helper, "dynamic://Bundle." + bundleKey);

            StringBuilder sb = new StringBuilder();
            foreach (var include in bundleManager.GetBundleIncludes(bundleKey))
            {
                var scriptUrl = include;
                if (string.IsNullOrEmpty(scriptUrl))
                    continue;

                if (scriptUrl.StartsWith("dynamic://", StringComparison.OrdinalIgnoreCase))
                {
                    var scriptName = scriptUrl.Substring(10);
                    scriptUrl = scriptManager.GetScriptInclude(scriptName);
                    scriptUrl = VirtualPathUtility.ToAbsolute(context, "~/DynJS.axd/" + scriptUrl);
                }
                else
                {
                    scriptUrl = BundleUtils.ExpandVersionVariable(hostEnvironment.WebRootPath, scriptUrl);
                    scriptUrl = VirtualPathUtility.ToAbsolute(context, scriptUrl);
                }

                var scripts = GetIncludedScripts(context);

                if (!scripts.Contains(scriptUrl))
                {
                    scripts.Add(scriptUrl);
                    sb.AppendLine(string.Format(CultureInfo.InvariantCulture, 
                        "    <script src=\"{0}\" type=\"text/javascript\"></script>\n",
                        WebUtility.HtmlEncode(contentHashCache.ResolveWithHash(context.Request.PathBase, scriptUrl))));
                }
            }
            
            return new HtmlString(sb.ToString());
        }

        const string IncludedScriptsKey = "IncludedScripts";

        private static HashSet<string> GetIncludedScripts(HttpContext context)
        {
            HashSet<string> scripts = (HashSet<string>)context.Items[IncludedScriptsKey];
            if (scripts == null)
            {
                scripts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                context.Items[IncludedScriptsKey] = scripts;
            }

            return scripts;
        }

        const string IncludedCssListKey = "IncludedStylesheets";

        private static HashSet<string> GetIncludedCssList(HttpContext context)
        {
            HashSet<string> styleSheets = (HashSet<string>)context.Items[IncludedCssListKey];
            if (styleSheets == null)
            {
                styleSheets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                context.Items[IncludedCssListKey] = styleSheets;
            }

            return styleSheets;
        }


        public static string GetLocalTextContent(this IHtmlHelper page, string package, bool isPending = false)
        {
            string languageId = CultureInfo.CurrentUICulture.Name.TrimToNull() ?? "invariant";
            string scriptName = Web.LocalTextScript.GetScriptName(package, languageId, isPending);
            var scriptManager = page.ViewContext.HttpContext.RequestServices.GetRequiredService<IDynamicScriptManager>();
            scriptManager.IfNotRegistered(scriptName, () =>
            {
                var services = page.ViewContext.HttpContext.RequestServices;
                var registry = services.GetRequiredService<ILocalTextRegistry>();
                var packages = services.GetRequiredService<IOptions<LocalTextPackages>>();
                return new LocalTextScript(registry, package, packages.Value[package], languageId, isPending);
            });

            return scriptManager.GetScriptText(scriptName);
        }

        public static string GetLocalTextInclude(this IHtmlHelper page, string package, bool isPending = false)
        {
            string languageId = CultureInfo.CurrentUICulture.Name.TrimToNull() ?? "invariant";
            string scriptName = Web.LocalTextScript.GetScriptName(package, languageId, isPending);
            var scriptManager = page.ViewContext.HttpContext.RequestServices.GetRequiredService<IDynamicScriptManager>();
            scriptManager.IfNotRegistered(scriptName, () =>
            {
                var services = page.ViewContext.HttpContext.RequestServices;
                var registry = services.GetRequiredService<ILocalTextRegistry>();
                var packages = services.GetRequiredService<IOptions<LocalTextPackages>>();
                return new LocalTextScript(registry, package, packages.Value[package], languageId, isPending);
            });

            return scriptManager.GetScriptInclude(scriptName);
        }

        public static HtmlString LocalTextScript(this IHtmlHelper page, string package, bool isPending = false)
        {
            string languageId = CultureInfo.CurrentUICulture.Name.TrimToNull() ?? "invariant";
            string scriptName = Web.LocalTextScript.GetScriptName(package, languageId, isPending);
            var scriptManager = page.ViewContext.HttpContext.RequestServices.GetRequiredService<IDynamicScriptManager>();
            scriptManager.IfNotRegistered(scriptName, () =>
            {
                var services = page.ViewContext.HttpContext.RequestServices;
                var registry = services.GetRequiredService<ILocalTextRegistry>();
                var packages = services.GetRequiredService<IOptions<LocalTextPackages>>();
                return new LocalTextScript(registry, package, packages.Value[package], languageId, isPending);
            });

            return Script(page, "dynamic://" + scriptName);
        }
    }
}