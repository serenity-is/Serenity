using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Serenity.Web.Html
{
    public static class HtmlScriptExtensions
    {
        public static IHtmlString IncludeCss(this HtmlHelper helper, string cssUrl)
        {
            return new HtmlString(string.Format("<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\"/>\r\n",
                JSHelper.ResolveCssVer(cssUrl)));
        }

        public static IHtmlString IncludeScriptHere(this HtmlHelper helper, string includeJS)
        {
            if (helper == null)
                throw new ArgumentNullException("helper");
            if (String.IsNullOrEmpty(includeJS))
                throw new ArgumentNullException("includeJS");

            var context = helper.ViewContext.HttpContext;

            var script = ScriptBundleManager.GetScriptBundle(includeJS);
            var scripts = GetRegisteredScripts(context);

            if (!scripts.Priorities.ContainsKey(script) ||
                scripts.Priorities[script] != Int32.MinValue)
            {
                scripts.Priorities[script] = Int32.MinValue;

                return new HtmlString(String.Format("<script src=\"{0}\" type=\"text/javascript\"></script>\r\n", 
                    JSHelper.ResolveJsVer(HttpUtility.HtmlAttributeEncode(script))));
            }
            else
                return new HtmlString("");
        }

        private class RegisteredScripts
        {
            public int CurrentLevel = 0;
            public Dictionary<string, int> Priorities = new Dictionary<string, int>();
        }

        const string RegisteredScriptsKey = "RegisteredScripts";

        private static RegisteredScripts GetRegisteredScripts(HttpContextBase context)
        {
            RegisteredScripts scripts = (RegisteredScripts)context.Items[RegisteredScriptsKey];
            if (scripts == null)
            {
                scripts = new RegisteredScripts();
                context.Items[RegisteredScriptsKey] = scripts;
            }

            return scripts;
        }

        /// <summary>
        ///   Includes given js file into page</summary>
        /// <param name="page">
        ///   Page in which js file to be included (required).</param>
        /// <param name="includeJS">
        ///   js file name (required). "~/js/include.js" can be set as relative path</param>
        public static void IncludeScript(this HtmlHelper helper, string includeJS)
        {
            if (helper == null)
                throw new ArgumentNullException("helper");
            if (String.IsNullOrEmpty(includeJS))
                throw new ArgumentNullException("includeJS");

            var context = helper.ViewContext.HttpContext;

            var script = ScriptBundleManager.GetScriptBundle(includeJS);
            var scripts = GetRegisteredScripts(context);

            if (!scripts.Priorities.ContainsKey(script))
                scripts.Priorities[script] = -scripts.CurrentLevel * 1024 + scripts.Priorities.Count;
        }

        public static void IncreaseScriptPriority(this HtmlHelper page)
        {
            var scripts = GetRegisteredScripts(page.ViewContext.RequestContext.HttpContext);
            scripts.CurrentLevel = scripts.CurrentLevel + 1;
        }

        public static IHtmlString GenerateScriptIncludes(this HtmlHelper page)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            var scripts = GetRegisteredScripts(page.ViewContext.HttpContext);
            if (scripts.Priorities.Count > 0)
            {
                var list = new List<string>();
                foreach (var k in scripts.Priorities)
                    if (k.Value != Int32.MinValue)
                        list.Add(k.Key);
                list.Sort((x, y) => scripts.Priorities[x] - scripts.Priorities[y]);

                StringBuilder sb = new StringBuilder(list.Count * 50);
                foreach (var script in list)
                    sb.AppendFormat("<script src=\"{0}\" type=\"text/javascript\"></script>\r\n", 
                        JSHelper.ResolveJsVer(HttpUtility.HtmlAttributeEncode(script)));

                return new HtmlString(sb.ToString());
            }
            else
                return null;
        }

        public static void IncludeJQ(this HtmlHelper page, params string[] scripts)
        {
            IncludeScript(page, "~/js/jquery/jquery.js");
            foreach (var name in scripts)
                IncludeScript(page, "~/js/jquery/jquery." + name + ".js");
        }

        public static void IncludeNT(this HtmlHelper page, params string[] scripts)
        {
            IncludeScript(page, "~/js/jquery/jquery.js");
            foreach (var name in scripts)
                IncludeScript(page, "~/js/nt/nt." + name + ".js");
        }

        public static void IncludeUI(this HtmlHelper page, params string[] scripts)
        {
            IncludeScript(page, "~/js/jquery/jquery.js");
            IncludeScript(page, "~/js/jquery/ui/jquery.ui.core.js");
            IncludeScript(page, "~/js/jquery/ui/jquery.ui.widget.js");
            IncludeScript(page, "~/js/jquery/ui/jquery.ui.button.js");
            IncludeScript(page, "~/js/jquery/ui/jquery.ui.position.js");
            IncludeScript(page, "~/js/jquery/ui/jquery.ui.mouse.js");
            foreach (var name in scripts)
                IncludeScript(page, "~/js/jquery/ui/jquery.ui." + name + ".js");
        }

        public static MvcHtmlString ScriptOnReady(this HtmlHelper page, string scriptBlock, params object[] jsonParameters)
        {
            var prm = new object[jsonParameters.Length];
            for (var i = 0; i < jsonParameters.Length; i++)
                prm[i] = jsonParameters[i].ToJsonString();

            return MvcHtmlString.Create(
                "<script type=\"text/javascript\">\n//<![CDATA[\n" +
                "(window.onDocumentReady || jQuery)(function($) {\n" +
                    String.Format(scriptBlock, prm) +
                "});\n" +
                "\n//]]></script>\n");
        }

        public static void IncludeDynamic(this HtmlHelper page, IScriptName script)
        {
            IncludeDynamic(page, script.ScriptName);
        }

        public static void IncludeDynamic(this HtmlHelper page, string name)
        {
            IncludeScript(page, "~/DynJS.axd/" + DynamicScriptManager.GetScriptInclude(name));
        }

        public static string GetLocalTextInclude(this HtmlHelper page, string package)
        {
            int languageId = (int)LocalText.ContextLanguageID;
            bool isPending = LocalText.ContextPending;
            string scriptName = LocalTextScript.GetScriptName(package, languageId, isPending);
            DynamicScriptManager.IfNotRegistered(scriptName, () =>
            {
                var script = new LocalTextScript(package, languageId, isPending);
                DynamicScriptManager.Register(script);
            });
            return scriptName;
        }

        public static void IncludeLocalInit(this HtmlHelper page)
        {
            int languageId = (int)LocalText.ContextLanguageID;
            string scriptName = LocalInitScript.GetScriptName(languageId);
            DynamicScriptManager.IfNotRegistered(scriptName, () =>
            {
                var script = new LocalInitScript(languageId);
                DynamicScriptManager.Register(script);
            });
            IncludeDynamic(page, scriptName);
        }
    }
}