using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Serenity.Data;
using System.Collections.Specialized;
using System.Configuration;

namespace Serenity.Web
{
    /// <summary>
    ///   Static class which contains javascript helper functions</summary>
    public static class JSHelper
    {
        private static string _versionPrefix;
        private static string _cssVersionSuffix;
        private static string _jsVersionSuffix;

        static JSHelper()
        {
            var settings = ConfigurationManager.AppSettings;
            int version = Convert.ToInt32(settings.Get("BuildVersionNumber").TrimToNull() ?? "0");
            _versionPrefix = "v" + version.ToString() + "__";
            _cssVersionSuffix = "?v" + version.ToString();
            _jsVersionSuffix = "v" + version.ToString();
        }

        public static void ScriptsChanged()
        {
            string version = 
                TemporaryFileHelper.RandomFileCode();
            _versionPrefix = "v" + version  + "__";
            _cssVersionSuffix = "?v" + version;
            _jsVersionSuffix = "v" + version;

            // TODO: ScriptBundleManager.ScriptsChanged();
        }

        public static string CssVersionSuffix
        {
            get { return _cssVersionSuffix; }
        }

        /// <summary>
        ///   Views a confimation box which contains given message  (confirm(message))</summary>
        /// <param name="message">
        ///   Messagge to be viewed (required).</param>
        /// <returns>
        ///   return confirm('message')</returns>
        public static string Confirmation(string message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            return String.Format("return confirm('{0}');", message.Replace("'", "''"));
        }

        /// <summary>
        ///   Includes given js file into page</summary>
        /// <param name="page">
        ///   Page in which js file to be included (required).</param>
        /// <param name="includeJS">
        ///   js file name (required). "~/js/include.js" can be set as relative path</param>
        public static void Include(Page page, string includeJS)
        {
            if (page == null)
                throw new ArgumentNullException("page");
            if (String.IsNullOrEmpty(includeJS))
                throw new ArgumentNullException("includeJS");

            includeJS = ScriptBundleManager.GetScriptBundle(includeJS);

            if (!page.ClientScript.IsClientScriptIncludeRegistered(typeof(Page), includeJS))
                page.ClientScript.RegisterClientScriptInclude(typeof(Page), includeJS, includeJS);
        }

        /// <summary>
        ///   Includes given js files into page</summary>
        /// <param name="page">
        ///   Page in which js file to be included (required).</param>
        /// <param name="includeJS">
        ///   an array includes given js file names (required). "~/js/include.js" can be set as relative path.</param>
        public static void Include(Page page, params string[] includeJS)
        {
            if (includeJS == null)
                throw new ArgumentNullException("includeJS");

            foreach (string s in includeJS)
                Include(page, s);
        }

        public static string ResolveCssVer(string cssUrl)
        {
            if (cssUrl.IsEmptyOrNull())
                throw new ArgumentNullException("cssUrl");

            cssUrl = VirtualPathUtility.ToAbsolute(cssUrl);
            cssUrl += _cssVersionSuffix;
            return cssUrl;
        }

        public static string ResolveJsVer(string jsUrl)
        {
            if (jsUrl.IsEmptyOrNull())
                throw new ArgumentNullException("jsUrl");

            jsUrl = VirtualPathUtility.ToAbsolute(jsUrl);

            if (jsUrl.IndexOf("DynJS.axd", StringComparison.OrdinalIgnoreCase) >= 0)
                return jsUrl;

            if (jsUrl.Contains("?"))
            {
                if (jsUrl.EndsWith("?"))
                    return jsUrl + _jsVersionSuffix;
                else
                    return jsUrl + "&" + _jsVersionSuffix;
            }
            else
                return jsUrl + "?" + _jsVersionSuffix;
        }

        /// <summary>
        ///   Includes css file into page. if css link being included , the link doesn't be included again.
        /// </summary>
        /// <param name="page">
        ///   Page in which css file to be included (required).</param>
        /// <param name="cssUrl">
        ///   CSS file url (required). "~/css/include.css" can be set as relative path.</param>
        public static void IncludeCss(Page page, string cssUrl)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            if (cssUrl.IsEmptyOrNull())
                throw new ArgumentNullException("cssUrl");

            cssUrl = ResolveCssVer(cssUrl);

            foreach (var control in page.Header.Controls)
            {
                HtmlLink cssControl = control as HtmlLink;
                if (cssControl != null &&
                    cssControl.Href.Equals(cssUrl, StringComparison.OrdinalIgnoreCase) &&
                    cssControl.Attributes["rel"] == "stylesheet" &&
                    cssControl.Attributes["type"] == "text/css")
                    return;
            }

            HtmlLink css = new HtmlLink();
            css.Href = cssUrl;
            css.Attributes["rel"] = "stylesheet";
            css.Attributes["type"] = "text/css";
            page.Header.Controls.Add(css);
        }


        /// <summary>
        ///   Includes print css file into page. if print css link being included , the link doesn't be included again.</summary>
        /// <param name="page">
        ///   Page in which css file to be included (required).</param>
        /// <param name="cssUrl">
        ///   css file url (required). "~/css/includeprint.css" can be set as relative path .</param>
        public static void IncludePrintCss(Page page, string cssUrl)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            if (cssUrl.IsEmptyOrNull())
                throw new ArgumentNullException("cssUrl");

            cssUrl = page.ResolveUrl(cssUrl);


            foreach (var control in page.Header.Controls)
            {
                HtmlLink cssControl = control as HtmlLink;
                if (cssControl != null &&
                    cssControl.Href.Equals(cssUrl, StringComparison.OrdinalIgnoreCase) &&
                    cssControl.Attributes["rel"] == "stylesheet" &&
                    cssControl.Attributes["type"] == "text/css" &&
                    cssControl.Attributes["media"] == "print")
                    return;
            }

            HtmlLink css = new HtmlLink();
            css.Href = cssUrl;
            css.Attributes["rel"] = "stylesheet";
            css.Attributes["type"] = "text/css";
            css.Attributes["media"] = "print";
            page.Header.Controls.Add(css);
        }

        /// <summary>
        ///   Includes print rss file into page. if print rss link being included , the link doesn't be included again.</summary>
        /// <param name="rssTitle">
        ///   RSS title (optional)</param>
        /// <param name="page">
        ///   Page in which rss file to be included (required).</param>
        /// <param name="rssUrl">
        ///   rss file url (required). "~/css/include.rss" can be set as relative path .</param>
        public static void IncludeRss(Page page, string rssUrl, string rssTitle)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            if (rssUrl.IsEmptyOrNull())
                throw new ArgumentNullException("rssUrl");

            rssUrl = page.ResolveUrl(rssUrl);


            foreach (var control in page.Header.Controls)
            {
                HtmlLink rssControl = control as HtmlLink;
                if (rssControl != null &&
                    rssControl.Href.Equals(rssUrl, StringComparison.OrdinalIgnoreCase) &&
                    rssControl.Attributes["rel"] == "alternate" &&
                    rssControl.Attributes["type"] == "application/rss+xml" &&
                    rssControl.Attributes["title"] == rssTitle)
                    return;
            }

            HtmlLink rss = new HtmlLink();
            rss.Href = rssUrl;
            rss.Attributes["rel"] = "alternate";
            rss.Attributes["type"] = "application/rss+xml";
            rss.Attributes["title"] = rssTitle;
            page.Header.Controls.Add(rss);
        }
    }
}
