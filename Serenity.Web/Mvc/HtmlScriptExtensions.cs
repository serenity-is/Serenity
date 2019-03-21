using Serenity.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
#if ASPNETCORE
using System.Net;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using IHtmlString = Microsoft.AspNetCore.Html.HtmlString;
using HtmlHelper = Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper;
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;
using VirtualPathUtility = System.Web.VirtualPathUtility;
#else
using System.Web;
using System.Web.Mvc;
#endif

namespace Serenity.Web
{
    public static class HtmlScriptExtensions
    {
        public static HtmlString Stylesheet(this HtmlHelper helper, string cssUrl)
        {
            if (helper == null)
                throw new ArgumentNullException("helper");

            if (String.IsNullOrEmpty(cssUrl))
                throw new ArgumentNullException("cssUrl");

            var context = helper.ViewContext.HttpContext;

            var css = CssBundleManager.GetCssBundle(cssUrl);
            var included = GetIncludedCssList(context);

            if (!included.Contains(css))
            {
                included.Add(css);

                return new HtmlString(String.Format("    <link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\"/>\n",
#if ASPNETCORE
                    WebUtility.HtmlEncode(ContentHashCache.ResolveWithHash(css))));
#else
                    HttpUtility.HtmlAttributeEncode(ContentHashCache.ResolveWithHash(css))));
#endif
            }
            else
                return new HtmlString("");
        }

        public static IHtmlString StyleBundle(this HtmlHelper helper, string bundleKey)
        {
            if (helper == null)
                throw new ArgumentNullException("helper");

            if (String.IsNullOrEmpty(bundleKey))
                throw new ArgumentNullException("bundleKey");

            var context = helper.ViewContext.HttpContext;
            if (!CssBundleManager.IsEnabled)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var include in CssBundleManager.GetBundleIncludes(bundleKey))
                {
                    var cssUrl = include;
                    if (string.IsNullOrEmpty(cssUrl))
                        continue;

                    if (cssUrl != null && cssUrl.StartsWith("dynamic://", StringComparison.OrdinalIgnoreCase))
                    {
                        var scriptName = cssUrl.Substring(10);
                        cssUrl = DynamicScriptManager.GetScriptInclude(scriptName, ".css");
                        cssUrl = VirtualPathUtility.ToAbsolute("~/DynJS.axd/" + cssUrl);
                    }
                    else
                    {
                        cssUrl = BundleUtils.ExpandVersionVariable(cssUrl);
                        cssUrl = VirtualPathUtility.ToAbsolute(cssUrl);
                    }

                    var cssList = GetIncludedCssList(context);

                    if (!cssList.Contains(cssUrl))
                    {
                        cssList.Add(cssUrl);
                        sb.AppendLine(String.Format("    <link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\"/>\n",
#if ASPNETCORE
                            WebUtility.HtmlEncode(ContentHashCache.ResolveWithHash(cssUrl))));
#else
                            HttpUtility.HtmlAttributeEncode(ContentHashCache.ResolveWithHash(cssUrl))));
#endif
                    }
                }

                return new HtmlString(sb.ToString());
            }

            return Stylesheet(helper, "dynamic://CssBundle." + bundleKey);
        }

        public static IHtmlString Script(this HtmlHelper helper, string includeJS)
        {
            if (helper == null)
                throw new ArgumentNullException("helper");

            if (String.IsNullOrEmpty(includeJS))
                throw new ArgumentNullException("includeJS");

            var context = helper.ViewContext.HttpContext;

            var script = ScriptBundleManager.GetScriptBundle(includeJS);
            var scripts = GetIncludedScripts(context);

            if (!scripts.Contains(script))
            {
                scripts.Add(script);

                return new HtmlString(String.Format("    <script src=\"{0}\" type=\"text/javascript\"></script>\n",
#if ASPNETCORE
                    WebUtility.HtmlEncode(ContentHashCache.ResolveWithHash(script))));
#else
                    HttpUtility.HtmlAttributeEncode(ContentHashCache.ResolveWithHash(script))));
#endif
            }
            else
                return new HtmlString("");
        }

        public static IHtmlString ScriptBundle(this HtmlHelper helper, string bundleKey)
        {
            if (helper == null)
                throw new ArgumentNullException("helper");

            if (String.IsNullOrEmpty(bundleKey))
                throw new ArgumentNullException("bundleKey");

            var context = helper.ViewContext.HttpContext;
            if (!ScriptBundleManager.IsEnabled)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var include in ScriptBundleManager.GetBundleIncludes(bundleKey))
                {
                    var scriptUrl = include;
                    if (string.IsNullOrEmpty(scriptUrl))
                        continue;

                    if (scriptUrl != null && scriptUrl.StartsWith("dynamic://", StringComparison.OrdinalIgnoreCase))
                    {
                        var scriptName = scriptUrl.Substring(10);
                        scriptUrl = DynamicScriptManager.GetScriptInclude(scriptName);
                        scriptUrl = VirtualPathUtility.ToAbsolute("~/DynJS.axd/" + scriptUrl);
                    }
                    else
                    {
                        scriptUrl = BundleUtils.ExpandVersionVariable(scriptUrl);
                        scriptUrl = VirtualPathUtility.ToAbsolute(scriptUrl);
                    }

                    var scripts = GetIncludedScripts(context);

                    if (!scripts.Contains(scriptUrl))
                    {
                        scripts.Add(scriptUrl);
                        sb.AppendLine(String.Format("    <script src=\"{0}\" type=\"text/javascript\"></script>\n",
#if ASPNETCORE
                            WebUtility.HtmlEncode(ContentHashCache.ResolveWithHash(scriptUrl))));
#else
                            HttpUtility.HtmlAttributeEncode(ContentHashCache.ResolveWithHash(scriptUrl))));
#endif
                    }
                }

                return new HtmlString(sb.ToString());
            }

            return Script(helper, "dynamic://Bundle." + bundleKey);
        }

        const string IncludedScriptsKey = "IncludedScripts";

        private static HashSet<string> GetIncludedScripts(HttpContextBase context)
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

        private static HashSet<string> GetIncludedCssList(HttpContextBase context)
        {
            HashSet<string> styleSheets = (HashSet<string>)context.Items[IncludedCssListKey];
            if (styleSheets == null)
            {
                styleSheets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                context.Items[IncludedCssListKey] = styleSheets;
            }

            return styleSheets;
        }


        public static string GetLocalTextContent(this HtmlHelper page, string package)
        {
            string languageId = CultureInfo.CurrentUICulture.Name.TrimToNull() ?? "invariant";
            var context = Dependency.TryResolve<ILocalTextContext>();
            var isPending = context != null && context.IsApprovalMode;
            string scriptName = Serenity.Web.LocalTextScript.GetScriptName(package, languageId, isPending);
            DynamicScriptManager.IfNotRegistered(scriptName, () =>
            {
                var script = new LocalTextScript(package, (string)languageId, isPending);
                DynamicScriptManager.Register(script);
            });

            return DynamicScriptManager.GetScriptText(scriptName);
        }

        public static string GetLocalTextInclude(this HtmlHelper page, string package)
        {
            string languageId = CultureInfo.CurrentUICulture.Name.TrimToNull() ?? "invariant";
            var context = Dependency.TryResolve<ILocalTextContext>();
            var isPending = context != null && context.IsApprovalMode;
            string scriptName = Serenity.Web.LocalTextScript.GetScriptName(package, languageId, isPending);
            DynamicScriptManager.IfNotRegistered(scriptName, () =>
            {
                var script = new Serenity.Web.LocalTextScript(package, (string)languageId, isPending);
                DynamicScriptManager.Register(script);
            });

            return DynamicScriptManager.GetScriptInclude(scriptName);
        }

        public static IHtmlString LocalTextScript(this HtmlHelper page, string package)
        {
            string languageId = CultureInfo.CurrentUICulture.Name.TrimToNull() ?? "invariant";
            var context = Dependency.TryResolve<ILocalTextContext>();
            var isPending = context != null && context.IsApprovalMode;
            string scriptName = Serenity.Web.LocalTextScript.GetScriptName(package, languageId, isPending);
            DynamicScriptManager.IfNotRegistered(scriptName, () =>
            {
                var script = new LocalTextScript(package, (string)languageId, isPending);
                DynamicScriptManager.Register(script);
            });

            return Script(page, "dynamic://" + scriptName);
        }
    }
}