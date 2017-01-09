#if ASPNETCORE
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace Serenity.Web
{
    public static class TemplateHelper
    {
        private static ActionContext GetActionContext(IServiceProvider serviceProvider)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = serviceProvider;
            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }

        public static string RenderViewToString(IServiceProvider serviceProvider, string viewName, object model)
        {
            var actionContext = GetActionContext(serviceProvider);
            var viewEngine = serviceProvider.GetService<IRazorViewEngine>();
            var viewEngineResult = viewEngine.FindView(actionContext, viewName, false);

            if (!viewEngineResult.Success)
                throw new InvalidOperationException(string.Format("Couldn't find view '{0}'", viewName));

            var viewData = new ViewDataDictionary(
                metadataProvider: new EmptyModelMetadataProvider(),
                modelState: new ModelStateDictionary());

            viewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                var engine = serviceProvider.GetService<ICompositeViewEngine>();

                var viewContext = new ViewContext(actionContext, viewEngineResult.View, viewData, 
                    new TempDataDictionary(actionContext.HttpContext, serviceProvider.GetService<ITempDataProvider>()), 
                        sw, new HtmlHelperOptions());

                var t = viewEngineResult.View.RenderAsync(viewContext);
                t.Wait();

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
#else
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
    using System.Web.Hosting;

    public interface ITemplateRenderer
    {
        string Render(string controlPath, ViewDataDictionary data);
    }

    public static class TemplateHelper
    {
        public static string RenderViewToString(string controlPath, ViewDataDictionary data)
        {
            var renderer = Dependency.TryResolve<ITemplateRenderer>();
            if (renderer != null)
                return renderer.Render(controlPath, data);

            StringBuilder sb = new StringBuilder();

            if (controlPath != null && controlPath.EndsWith(".cshtml", System.StringComparison.InvariantCultureIgnoreCase))
            {
                data = data ?? new ViewDataDictionary(new Dictionary<string, object>());
                var httpBase = new HttpContextWrapper(HttpContext.Current);

                var controller = new FakeController();
                var route = new RouteData();
                route.Values.Add("controller", "something");
                var controllerContext = new ControllerContext(httpBase, route, controller);

                var actualPath = controlPath;
                if (actualPath.StartsWith("~/"))
                {
                    actualPath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath,
                        actualPath.Substring(2).Replace("/", "\\"));
                }

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
#endif