using Serenity.Web.MvcFakes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;

namespace Serenity.Web
{
    public static class TemplateHelper
    {
        public static string RenderViewToString(string controlPath, ViewDataDictionary data)
        {
            StringBuilder sb = new StringBuilder();

            if (controlPath != null && controlPath.EndsWith(".cshtml", System.StringComparison.InvariantCultureIgnoreCase))
            {
                data = data ?? new ViewDataDictionary(new Dictionary<string, object>());
                var httpBase = new HttpContextWrapper(HttpContext.Current);
                var controller = new FakeController();
                var controllerContext = new ControllerContext(httpBase, new RouteData(), controller);

                var actualPath = controlPath;
                if (actualPath.StartsWith("~/"))
                    actualPath = Path.Combine(HttpRuntime.AppDomainAppPath, actualPath.Substring(2).Replace("/", "\\"));

                if (!File.Exists(actualPath))
                    throw new ArgumentOutOfRangeException("controlPath", String.Format("controlPath: {0}", actualPath));

                var view = new RazorView(controllerContext, controlPath, null, false, null, null);
                using (StringWriter sw = new StringWriter(sb))
                {
                    var viewContext = new ViewContext(controllerContext, view, data, new TempDataDictionary(), sw);
                    view.Render(viewContext, sw);
                }
            }
            else
            {
                ViewPage vp = new ViewPage { ViewData = data };
                Control control = vp.LoadControl(controlPath);
                vp.Controls.Add(control);
                using (StringWriter sw = new StringWriter(sb))
                using (HtmlTextWriter tw = new HtmlTextWriter(sw))
                {
                    vp.RenderControl(tw);
                }
            }

            return sb.ToString();
        }

        public static string RenderTemplate<T>(string templatePath, T model)
        {
            return RenderViewToString(templatePath, new ViewDataDictionary<T>(model));
        }
    }
}