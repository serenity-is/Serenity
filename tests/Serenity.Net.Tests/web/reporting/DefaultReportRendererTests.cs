namespace Serenity.Reporting;

public class DefaultReportRendererTests
{
    [ReportDesign("TestDesign")]
    private class DesignReport : IReport, IReportWithAdditionalData
    {
        public object GetData() => "model";
        public IDictionary<string, object?> GetAdditionalData() => new Dictionary<string, object?> { ["k"] = "v" };
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

    private class FakeExcelRenderer : IDataReportExcelRenderer
    {
        public byte[] Render(IDataOnlyReport report) => [1, 2];
    }

    private class FakePdfRenderer : IHtmlReportPdfRenderer
    {
        public byte[] Render(IReport report, ReportRenderOptions renderOptions) => [3, 4];
    }

    private static DefaultReportRenderer Create()
    {
        return new DefaultReportRenderer(new FakeExcelRenderer(), new FakePdfRenderer(),
            new ServiceCollection().BuildServiceProvider());
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
}
