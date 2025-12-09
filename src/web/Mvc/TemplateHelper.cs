using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace Serenity.Web;

/// <summary>
/// Helper methods for CSHTML templating
/// </summary>
public static class TemplateHelper
{
    /// <summary>
    /// Renders a CSHTML view to string
    /// </summary>
    /// <param name="serviceProvider">Service provider</param>
    /// <param name="viewName">View name</param>
    /// <param name="model">Model object, can also be ViewDataDictionary containing the actual model</param>
    /// <param name="beforeRender">An optional callback which will be passed the view context before rendering starts</param>
    /// <exception cref="InvalidOperationException"></exception>
    public static string RenderViewToString(IServiceProvider serviceProvider, string viewName, object model,
        Action<ViewContext> beforeRender = null)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        var viewEngine = serviceProvider.GetRequiredService<IRazorViewEngine>();
        var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
        var tempDataProvider = serviceProvider.GetRequiredService<ITempDataProvider>();

        var viewEngineResult = viewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: false);

        ActionContext actionContext;
        if (httpContextAccessor?.HttpContext?.GetEndpoint() is Endpoint endpoint &&
            endpoint.Metadata.GetMetadata<ActionDescriptor>() is ActionDescriptor actionDescriptor)
        {
            actionContext = new ActionContext(httpContextAccessor.HttpContext,
                httpContextAccessor.HttpContext.GetRouteData() ?? new RouteData(),
                actionDescriptor);
        }
        else
        {

            var httpContext = serviceProvider?.GetService<IHttpContextAccessor>()?.HttpContext ??
                new DefaultHttpContext
                {
                    RequestServices = serviceProvider
                };

            actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }

        if (viewEngineResult.View == null || (!viewEngineResult.Success))
            throw new ArgumentNullException($"Unable to find view '{viewName}'");

        if (model is not ViewDataDictionary viewData)
        {
            var metadataProvider = serviceProvider.GetService<IModelMetadataProvider>()
                ?? serviceProvider.GetService<IModelMetadataProvider>() ??
                new EmptyModelMetadataProvider();

            viewData = new ViewDataDictionary(metadataProvider, actionContext.ModelState ?? new ModelStateDictionary())
            {
                Model = model
            };
        }

        var view = viewEngineResult.View;
        var tempData = new TempDataDictionary(actionContext.HttpContext, tempDataProvider);
        
        using var sw = new StringWriter();
        var viewContext = new ViewContext(actionContext, view, viewData, tempData, sw, new HtmlHelperOptions());
        
        beforeRender?.Invoke(viewContext);
        
        view.RenderAsync(viewContext).Wait();
        
        sw.Flush();
        return sw.GetStringBuilder().ToString();
    }
}