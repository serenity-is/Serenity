namespace Serenity.Reporting;

public class DefaultReportRendererTests
{
    [ReportDesign("TestDesign")]
    private class DesignReport : IReport, IReportWithAdditionalData
    {
        public object GetData() => "model";
        public IDictionary<string, object?> GetAdditionalData() => new Dictionary<string, object?> { ["k"] = "v" };
    }

    [ReportDesign("AsyncDesign")]
    private class AsyncDataReport : IReport
    {
        public object GetData() => throw new InvalidOperationException();
        public async Task<object?> GetDataAsync(System.Threading.CancellationToken cancellationToken)
        {
            await Task.Yield();
            return "async model";
        }
    }

    private class NoDesignReport : IReport
    {
        public object GetData() => "model";
    }

    private class DataOnlyReport : IDataOnlyReport
    {
        public object GetData() => "d";
        public List<ReportColumn> GetColumnList() => [];
    }

    private class ExternalReport : IExternalReport
    {
        public object GetData() => "https://example.com/report";
    }

    private class EmptyExternalReport : IExternalReport
    {
        public object GetData() => null!;
    }

    [ReportDesign("Custom")]
    private class CustomFileNameReport : IDataOnlyReport, ICustomFileName
    {
        public object GetData() => "d";
        public List<ReportColumn> GetColumnList() => [];
        public string GetFileName() => "custom_file";
    }

    [DisplayText("Report fallback", TextKey = "Site.RendererFileName")]
    private class LocalizedFileNameReport : IDataOnlyReport
    {
        public object GetData() => "d";
        public List<ReportColumn> GetColumnList() => [];
    }

    [DisplayText("Site.LiteralRendererFileName")]
    private class LiteralFileNameReport : IDataOnlyReport
    {
        public object GetData() => "d";
        public List<ReportColumn> GetColumnList() => [];
    }

    [DisplayName("Site.LegacyRendererFileName")]
    private class LegacyKeyFileNameReport : IDataOnlyReport
    {
        public object GetData() => "d";
        public List<ReportColumn> GetColumnList() => [];
    }

    private class FakeExcelRenderer : IDataReportExcelRenderer
    {
        public byte[] Render(IDataOnlyReport report) => [1, 2];
    }

    private class FakePdfRenderer : IHtmlReportPdfRenderer
    {
        public byte[] Render(IReport report, ReportRenderOptions renderOptions) => [3, 4];
    }

    private class StructuredReport : IStructuredReport
    {
        public async Task<StructuredReportResult> GetResultAsync(System.Threading.CancellationToken cancellationToken)
        {
            await Task.Yield();
            return new StructuredReportResult { EmptyText = "async" };
        }
    }

    private class FakeStructuredRenderer : IStructuredReportRenderer
    {
        public async Task<ReportRenderResult> RenderAsync(IStructuredReport report, ReportRenderOptions renderOptions,
            System.Threading.CancellationToken cancellationToken)
        {
            await Task.Yield();
            return new() { FileExtension = "." + renderOptions.ExportFormat };
        }
    }

    private class AsyncPdfRenderer : IHtmlReportPdfRenderer
    {
        public byte[] Render(IReport report, ReportRenderOptions renderOptions) => throw new InvalidOperationException();
        public async Task<byte[]> RenderAsync(IReport report, ReportRenderOptions renderOptions, System.Threading.CancellationToken cancellationToken)
        {
            await Task.Yield();
            return [5, 6];
        }
    }

    private class SyncInterceptor : IReportCallbackInterceptor
    {
        public List<string> Log { get; } = [];
        public ReportRenderResult InterceptCallback(ReportRenderOptions renderOptions, Func<ReportRenderOptions, ReportRenderResult> action)
        {
            Log.Add("before");
            try { return action(renderOptions); }
            finally { Log.Add("after"); }
        }
    }

    private static DefaultReportRenderer Create(ITextLocalizer? localizer = null)
    {
        return new DefaultReportRenderer(new FakeExcelRenderer(), new FakePdfRenderer(),
            new ServiceCollection().BuildServiceProvider(), localizer: localizer);
    }

    [Fact]
    public void Constructor_Throws_For_Null_Arguments()
    {
        var sp = new ServiceCollection().BuildServiceProvider();
        Assert.Throws<ArgumentNullException>(() => new DefaultReportRenderer(null!, new FakePdfRenderer(), sp));
        Assert.Throws<ArgumentNullException>(() => new DefaultReportRenderer(new FakeExcelRenderer(), null!, sp));
        Assert.Throws<ArgumentNullException>(() => new DefaultReportRenderer(new FakeExcelRenderer(), new FakePdfRenderer(), null!));
    }

    [Fact]
    public void Render_Throws_For_Null_Arguments()
    {
        var renderer = Create();
        Assert.Throws<ArgumentNullException>(() => renderer.Render(null!, new ReportRenderOptions()));
        Assert.Throws<ArgumentNullException>(() => renderer.Render(new DesignReport(), null!));
    }

    [Fact]
    public void Render_DataOnlyReport_Uses_Excel_Renderer()
    {
        var result = Create().Render(new DataOnlyReport(), new ReportRenderOptions());

        Assert.Equal([1, 2], result.ContentBytes);
        Assert.Equal(".xlsx", result.FileExtension);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.MimeType);
    }

    [Fact]
    public void Render_ExternalReport_Returns_Redirect()
    {
        var result = Create().Render(new ExternalReport(), new ReportRenderOptions());

        Assert.Equal("https://example.com/report", result.RedirectUri);
    }

    [Fact]
    public void Render_ExternalReport_Throws_When_Url_Empty()
    {
        Assert.Throws<InvalidProgramException>(() =>
            Create().Render(new EmptyExternalReport(), new ReportRenderOptions()));
    }

    [Fact]
    public void Render_Html_Preview_Mode_Returns_View_And_Model()
    {
        var result = Create().Render(new DesignReport(), new ReportRenderOptions { PreviewMode = true });

        Assert.Equal("TestDesign", result.ViewName);
        Assert.Equal("model", result.Model);
        Assert.Equal(false, result.ViewData["Printing"]);
        Assert.NotNull(result.ViewData["AdditionalData"]);
    }

    [Fact]
    public void Render_Html_Preview_Throws_Without_Design_Attribute()
    {
        Assert.Throws<InvalidOperationException>(() =>
            Create().Render(new NoDesignReport(), new ReportRenderOptions { PreviewMode = true }));
    }

    [Fact]
    public void Render_Pdf_Uses_Pdf_Renderer()
    {
        var result = Create().Render(new DesignReport(), new ReportRenderOptions { ExportFormat = "pdf" });

        Assert.Equal([3, 4], result.ContentBytes);
        Assert.Equal(".pdf", result.FileExtension);
        Assert.Equal("application/pdf", result.MimeType);
    }

    [Fact]
    public void Render_Unknown_Format_Throws()
    {
        Assert.Throws<NotImplementedException>(() =>
            Create().Render(new DesignReport(), new ReportRenderOptions { ExportFormat = "xlsx" }));
    }

    [Fact]
    public void Render_Uses_Custom_File_Name()
    {
        var result = Create().Render(new CustomFileNameReport(), new ReportRenderOptions());

        Assert.Equal("custom_file", result.FileName);
    }

    [Fact]
    public void Render_Localizes_DisplayText_File_Name_When_Localizer_Is_Available()
    {
        var renderer = Create(new MockTextLocalizer(key => key == "Site.RendererFileName" ?
            "Localized report" : null));

        var result = renderer.Render(new LocalizedFileNameReport(), new ReportRenderOptions());

        Assert.StartsWith("Localized report_", result.FileName);
    }

    [Fact]
    public void Render_Uses_DisplayText_Fallback_When_Localizer_Is_Null()
    {
        var result = Create().Render(new LocalizedFileNameReport(), new ReportRenderOptions());

        Assert.StartsWith("Report fallback_", result.FileName);
    }

    [Fact]
    public void Render_Does_Not_Infer_Key_Looking_DisplayText_File_Name()
    {
        var result = Create().Render(new LiteralFileNameReport(), new ReportRenderOptions());

        Assert.StartsWith("Site.LiteralRendererFileName_", result.FileName);
    }

    [Fact]
    public void Render_Localizes_Key_Looking_DisplayName_File_Name()
    {
        var renderer = Create(new MockTextLocalizer(key => key == "Site.LegacyRendererFileName" ?
            "Localized legacy report" : null));

        var result = renderer.Render(new LegacyKeyFileNameReport(), new ReportRenderOptions());

        Assert.StartsWith("Localized legacy report_", result.FileName);
    }

    [Fact]
    public void Structured_Report_Data_Is_Its_Result()
    {
        Assert.Equal("async", Assert.IsType<StructuredReportResult>(((IReport)new StructuredReport()).GetData()).EmptyText);
    }

    [Fact]
    public void Render_Uses_The_Structured_Report_Renderer_For_A_Structured_Report()
    {
        var services = new ServiceCollection().AddSingleton<IStructuredReportRenderer, FakeStructuredRenderer>().BuildServiceProvider();
        var renderer = new DefaultReportRenderer(new FakeExcelRenderer(), new FakePdfRenderer(), services);

        var result = renderer.Render(new StructuredReport(), new ReportRenderOptions { ExportFormat = "csv" });

        Assert.Equal(".csv", result.FileExtension);
    }

    [Fact]
    public void Render_Throws_For_A_Structured_Report_Without_A_Structured_Report_Renderer()
    {
        Assert.Throws<InvalidOperationException>(() => Create().Render(new StructuredReport(), new ReportRenderOptions()));
    }

    [Fact]
    public async Task RenderAsync_Renders_The_Pdf_Of_An_Html_Report_Asynchronously()
    {
        var renderer = new DefaultReportRenderer(new FakeExcelRenderer(), new AsyncPdfRenderer(), new ServiceCollection().BuildServiceProvider());

        var result = await renderer.RenderAsync(new DesignReport(), new ReportRenderOptions { ExportFormat = "PDF" }, TestContext.Current.CancellationToken);

        Assert.Equal([5, 6], result.ContentBytes);
        Assert.Equal(".pdf", result.FileExtension);
        Assert.Equal("application/pdf", result.MimeType);
    }

    [Fact]
    public async Task RenderAsync_Uses_Render_Of_A_Pdf_Renderer_That_Is_Not_Async()
    {
        var result = await Create().RenderAsync(new DesignReport(), new ReportRenderOptions { ExportFormat = "pdf" }, TestContext.Current.CancellationToken);
        Assert.Equal([3, 4], result.ContentBytes);
    }

    [Fact]
    public async Task RenderAsync_Renders_A_Structured_Report_Asynchronously()
    {
        var services = new ServiceCollection().AddSingleton<IStructuredReportRenderer, FakeStructuredRenderer>().BuildServiceProvider();
        var renderer = new DefaultReportRenderer(new FakeExcelRenderer(), new FakePdfRenderer(), services);

        var result = await renderer.RenderAsync(new StructuredReport(), new ReportRenderOptions { ExportFormat = "pdf" }, TestContext.Current.CancellationToken);

        Assert.Equal(".pdf", result.FileExtension);
        Assert.Equal("async", Assert.IsType<StructuredReportResult>(await ((IReport)new StructuredReport()).GetDataAsync(TestContext.Current.CancellationToken)).EmptyText);
        await Assert.ThrowsAsync<InvalidOperationException>(() => Create().RenderAsync(new StructuredReport(), new ReportRenderOptions(), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task RenderAsync_Renders_The_Other_Reports_As_Render_Does()
    {
        var renderer = Create();

        Assert.Equal([1, 2], (await renderer.RenderAsync(new DataOnlyReport(), new ReportRenderOptions { ExportFormat = "pdf" }, TestContext.Current.CancellationToken)).ContentBytes);
        Assert.Equal("https://example.com/report", (await renderer.RenderAsync(new ExternalReport(), new ReportRenderOptions(), TestContext.Current.CancellationToken)).RedirectUri);
        await Assert.ThrowsAsync<ArgumentNullException>(() => renderer.RenderAsync(null!, new ReportRenderOptions(), TestContext.Current.CancellationToken));
        await Assert.ThrowsAsync<ArgumentNullException>(() => renderer.RenderAsync(new DataOnlyReport(), null!, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task InterceptCallbackAsync_Runs_An_Async_Action_In_The_Sync_Interceptor_By_Default()
    {
        var interceptor = new SyncInterceptor();
        var result = await ((IReportCallbackInterceptor)interceptor).InterceptCallbackAsync(new ReportRenderOptions(), async options =>
        {
            await Task.Yield();
            interceptor.Log.Add("action");
            return new ReportRenderResult { FileExtension = ".x" };
        });

        Assert.Equal(".x", result.FileExtension);
        Assert.Equal(["before", "action", "after"], interceptor.Log);
    }

    [Fact]
    public async Task RenderAsync_Gets_The_Data_Of_An_Html_Report_Asynchronously()
    {
        var result = await Create().RenderAsync(new AsyncDataReport(), new ReportRenderOptions { PreviewMode = true }, TestContext.Current.CancellationToken);

        Assert.Equal("AsyncDesign", result.ViewName);
        Assert.Equal("async model", result.Model);
        Assert.Equal("model", (await Create().RenderAsync(new DesignReport(), new ReportRenderOptions { PreviewMode = true }, TestContext.Current.CancellationToken)).Model);
    }
}
