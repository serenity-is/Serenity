using Serenity.Reporting;
namespace Serenity.Extensions.Pages;

public class ReportControllerTests
{
    private class TestReport : IReport
    {
        public object? GetData() => null;
    }

    private class FakeReportFactory : IReportFactory
    {
        public string? LastKey { get; private set; }

        public IReport Create(string reportKey, string? reportParams, bool validatePermission = true)
        {
            LastKey = reportKey;
            return new TestReport();
        }

        public void SetParams(IReport report, string reportParams)
        {
        }
    }

    private class FakeReportRenderer : IReportRenderer
    {
        public ReportRenderResult Result { get; set; } = new()
        {
            ContentBytes = [1, 2, 3],
            FileExtension = ".xlsx",
            MimeType = "application/excel"
        };

        public ReportRenderResult Render(IReport report, ReportRenderOptions renderOptions) => Result;
    }

    private class FakeInterceptor : IReportCallbackInterceptor
    {
        public ReportRenderResult? Result { get; set; }

        public ReportRenderResult InterceptCallback(ReportRenderOptions renderOptions,
            Func<ReportRenderOptions, ReportRenderResult> action)
        {
            return Result ?? action(renderOptions);
        }
    }

    private static ReportController CreateController(FakeReportFactory factory, FakeReportRenderer renderer,
        IReportCallbackInterceptor? interceptor, out DefaultHttpContext httpContext)
    {
        httpContext = new DefaultHttpContext();
        return new ReportController(factory, renderer, interceptor)
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext }
        };
    }

    [Fact]
    public void Constructor_Throws_For_Null_Arguments()
    {
        var renderer = new FakeReportRenderer();
        Assert.Throws<ArgumentNullException>(() => new ReportController(null!, renderer));
        Assert.Throws<ArgumentNullException>(() => new ReportController(new FakeReportFactory(), null!));
    }

    [Fact]
    public void Render_Returns_File_Result()
    {
        var controller = CreateController(new FakeReportFactory(), new FakeReportRenderer(), null, out var httpContext);

        var result = controller.Render("Test", null!, "xlsx");

        var file = Assert.IsType<FileContentResult>(result);
        Assert.Equal([1, 2, 3], file.FileContents);
        Assert.Contains("inline", httpContext.Response.Headers.ContentDisposition.ToString());
    }

    [Fact]
    public void Download_Returns_Attachment_File_Result()
    {
        var controller = CreateController(new FakeReportFactory(), new FakeReportRenderer(), null, out var httpContext);

        var result = controller.Download("Test", null!, "xlsx");

        Assert.IsType<FileContentResult>(result);
        Assert.Contains("attachment", httpContext.Response.Headers.ContentDisposition.ToString());
    }

    [Fact]
    public void Execute_Uses_CallbackInterceptor_And_Redirects()
    {
        var interceptor = new FakeInterceptor { Result = new ReportRenderResult { RedirectUri = "/somewhere" } };
        var controller = CreateController(new FakeReportFactory(), new FakeReportRenderer(), interceptor, out _);

        var result = controller.Render("Test", null!, "xlsx");
        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal("/somewhere", redirect.Url);
    }

    [Fact]
    public void Execute_Returns_View_When_ViewName_Specified()
    {
        var interceptor = new FakeInterceptor
        {
            Result = new ReportRenderResult { ViewName = "MyView", Model = "model" }
        };
        interceptor.Result.ViewData["X"] = "Y";
        var controller = CreateController(new FakeReportFactory(), new FakeReportRenderer(), interceptor, out _);

        var result = controller.Render("Test", null!, "html");
        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal("MyView", view.ViewName);
        Assert.Equal("Y", controller.ViewData["X"]);
    }

    [Fact]
    public void Render_Uses_Provided_File_Name()
    {
        var renderer = new FakeReportRenderer
        {
            Result = new ReportRenderResult
            {
                ContentBytes = [1],
                FileName = "MyReport",
                FileExtension = ".pdf",
                MimeType = "application/pdf"
            }
        };
        var controller = CreateController(new FakeReportFactory(), renderer, null, out var httpContext);

        controller.Render("Test", null!, "pdf");

        Assert.Contains("MyReport.pdf", httpContext.Response.Headers.ContentDisposition.ToString());
    }

    [Fact]
    public void Retrieve_Returns_Result()
    {
        var controller = CreateController(new FakeReportFactory(), new FakeReportRenderer(), null, out _);
        var handler = new FakeRetrieveHandler();

        var result = controller.Retrieve(new ReportRetrieveRequest { ReportKey = "Test" }, handler);
        var typed = Assert.IsType<Result<ReportRetrieveResponse>>(result);
        Assert.NotNull(typed.Data);
    }

    private class FakeRetrieveHandler : IReportRetrieveHandler
    {
        public ReportRetrieveResponse Retrieve(ReportRetrieveRequest request) => new();
    }
}

