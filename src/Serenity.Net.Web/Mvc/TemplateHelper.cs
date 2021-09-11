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
using System.Globalization;
using System.IO;

namespace Serenity.Web
{
    public static class TemplateHelper
    {
        private static ActionContext GetActionContext(IServiceProvider serviceProvider)
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProvider
            };
            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }

        public static string RenderViewToString(IServiceProvider serviceProvider, string viewName, object model)
        {
            var actionContext = GetActionContext(serviceProvider);
            var viewEngine = serviceProvider.GetService<IRazorViewEngine>();
            var viewEngineResult = viewEngine.GetView(executingFilePath: viewName, viewPath: viewName, isMainPage: false);

            if (!viewEngineResult.Success)
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, 
                    "Couldn't find view '{0}'", viewName));

            var viewData = new ViewDataDictionary(
                metadataProvider: new EmptyModelMetadataProvider(),
                modelState: new ModelStateDictionary())
            {
                Model = model
            };

            using StringWriter sw = new();
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