using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Serenity.Web;

public class TemplateHelperTests
{
    private class FakeView(bool exists = true) : IView
    {
        public string Path => "/Test/View.cshtml";
        public bool Exists { get; } = exists;

        public Task RenderAsync(ViewContext context)
        {
            context.Writer.Write("RENDERED");
            return Task.CompletedTask;
        }
    }

    private class FakeViewEngine : IRazorViewEngine
    {
        public IView? View { get; set; } = new FakeView();
        public bool Success { get; set; } = true;

        public ViewEngineResult GetView(string? executingFilePath, string viewPath, bool isMainPage)
        {
            return Success && View != null
                ? ViewEngineResult.Found(viewPath, View)
                : ViewEngineResult.NotFound(viewPath, []);
        }

        public ViewEngineResult FindView(ActionContext actionContext, string viewName, bool isMainPage)
            => GetView(null, viewName, isMainPage);

        public RazorPageResult FindPage(ActionContext actionContext, string pageName)
            => new(pageName, []);

        public RazorPageResult GetPage(string? executingFilePath, string pagePath)
            => new(pagePath, []);

        public string GetAbsolutePath(string? executingFilePath, string pagePath) => pagePath;
    }

    private class FakeTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object?> LoadTempData(HttpContext context) => new Dictionary<string, object?>();
        public void SaveTempData(HttpContext context, IDictionary<string, object?> values)
        {
        }
    }

    private static IServiceProvider CreateServices(FakeViewEngine? viewEngine = null, bool withMetadata = true)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IRazorViewEngine>(viewEngine ?? new FakeViewEngine());
        services.AddSingleton<ITempDataProvider>(new FakeTempDataProvider());
        services.AddSingleton<IHttpContextAccessor>(new MockHttpContextAccessor());
        if (withMetadata)
            services.AddSingleton<IModelMetadataProvider>(new EmptyModelMetadataProvider());
        return services.BuildServiceProvider();
    }

    [Fact]
    public void RenderViewToString_Throws_For_Null_ServiceProvider()
    {
        Assert.Throws<ArgumentNullException>(() =>
            TemplateHelper.RenderViewToString(null!, "~/Views/Test.cshtml", null));
    }

    [Fact]
    public void RenderViewToString_Throws_When_View_Not_Found()
    {
        var services = CreateServices(new FakeViewEngine { Success = false });

        Assert.Throws<ArgumentNullException>(() =>
            TemplateHelper.RenderViewToString(services, "~/Views/Missing.cshtml", null));
    }

    [Fact]
    public void RenderViewToString_Renders_View_With_Model()
    {
        var services = CreateServices();

        var result = TemplateHelper.RenderViewToString(services, "~/Views/Test.cshtml", new { A = 1 });

        Assert.Equal("RENDERED", result);
    }

    [Fact]
    public void RenderViewToString_Invokes_BeforeRender()
    {
        var services = CreateServices();
        var beforeRenderCalled = false;

        var result = TemplateHelper.RenderViewToString(services, "~/Views/Test.cshtml", null, viewContext =>
        {
            beforeRenderCalled = true;
            viewContext.ViewData["X"] = "Y";
        });

        Assert.True(beforeRenderCalled);
        Assert.Equal("RENDERED", result);
    }

    [Fact]
    public void RenderViewToString_Uses_Provided_ViewData()
    {
        var services = CreateServices();
        var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());

        var result = TemplateHelper.RenderViewToString(services, "~/Views/Test.cshtml", viewData);

        Assert.Equal("RENDERED", result);
    }

    [Fact]
    public void RenderViewToString_Works_Without_Metadata_Provider()
    {
        var services = CreateServices(withMetadata: false);

        var result = TemplateHelper.RenderViewToString(services, "~/Views/Test.cshtml", new { A = 1 });

        Assert.Equal("RENDERED", result);
    }

    [Fact]
    public void RenderViewToString_Works_Without_HttpContext()
    {
        var services = new ServiceCollection()
            .AddSingleton<IRazorViewEngine>(new FakeViewEngine())
            .AddSingleton<ITempDataProvider>(new FakeTempDataProvider())
            .BuildServiceProvider();

        var result = TemplateHelper.RenderViewToString(services, "~/Views/Test.cshtml", null);

        Assert.Equal("RENDERED", result);
    }
}
