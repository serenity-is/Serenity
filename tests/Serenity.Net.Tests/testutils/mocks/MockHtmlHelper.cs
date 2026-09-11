using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Serenity.TestUtils;

public static class MockHtmlHelper
{
    private sealed class FakeView : IView
    {
        public string Path => "/Test/View.cshtml";
        public Task RenderAsync(ViewContext context) => Task.CompletedTask;
    }

    private sealed class FakeTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object?> LoadTempData(HttpContext context) => new Dictionary<string, object?>();
        public void SaveTempData(HttpContext context, IDictionary<string, object?> values)
        {
        }
    }

    public static IHtmlHelper Create(IServiceProvider services, Action<HttpContext>? configure = null,
        ViewDataDictionary? viewData = null)
    {
        var httpContext = new DefaultHttpContext { RequestServices = services };
        configure?.Invoke(httpContext);

        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        viewData ??= new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
        var viewContext = new ViewContext(actionContext, new FakeView(), viewData,
            new TempDataDictionary(httpContext, new FakeTempDataProvider()),
            new System.IO.StringWriter(), new HtmlHelperOptions());

        var mvcServices = new ServiceCollection();
        mvcServices.AddLogging();
        var diagnosticListener = new System.Diagnostics.DiagnosticListener("MockHtmlHelper");
        mvcServices.AddSingleton<System.Diagnostics.DiagnosticListener>(diagnosticListener);
        mvcServices.AddSingleton<System.Diagnostics.DiagnosticSource>(diagnosticListener);
        mvcServices.AddMvc();
        var helper = (HtmlHelper)mvcServices.BuildServiceProvider().GetRequiredService<IHtmlHelper>();
        typeof(HtmlHelper).GetProperty(nameof(HtmlHelper.ViewContext))!
            .GetSetMethod(nonPublic: true)!.Invoke(helper, [viewContext]);
        return helper;
    }

    public static IHtmlHelper Create(Action<HttpContext>? configure = null)
    {
        return Create(new ServiceCollection().BuildServiceProvider(), configure);
    }

    public static IHtmlHelper CreateWithItems(IDictionary<object, object?> items, IServiceProvider services)
    {
        return Create(services, context =>
        {
            foreach (var pair in items)
                context.Items[pair.Key] = pair.Value;
        });
    }
}
