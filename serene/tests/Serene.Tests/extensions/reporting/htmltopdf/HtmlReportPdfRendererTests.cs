using System.Net;

namespace Serenity.Reporting;

public class HtmlReportPdfRendererTests
{
    private class FakeConverter : IHtmlToPdfConverter
    {
        public IHtmlToPdfOptions? Options { get; private set; }
        public byte[] Bytes { get; set; } = [1, 2, 3];

        public byte[] Convert(IHtmlToPdfOptions options)
        {
            Options = options;
            return Bytes;
        }
    }

    private class FakeWkConverter : IWKHtmlToPdfConverter
    {
        public byte[] Convert(IHtmlToPdfOptions options) => [4, 5, 6];
        public string? GetExecutablePath() => "wkhtmltopdf";
    }

    private class TestRenderUrl : HtmlReportRenderUrl
    {
        public bool Disposed { get; private set; }
        public bool ThrowOnTemporaryFolders { get; set; }

        public override IEnumerable<string> GetTemporaryFolders()
        {
            if (ThrowOnTemporaryFolders)
                throw new InvalidOperationException("boom");
            return ["/tmp/a"];
        }

        protected override void Cleanup()
        {
            Disposed = true;
        }
    }

    private class FakeRenderUrlBuilder : IHtmlReportRenderUrlBuilder
    {
        public TestRenderUrl RenderUrl { get; set; } = new() { Url = "https://site/report" };

        public HtmlReportRenderUrl GetRenderUrl(IReport report, ReportRenderOptions renderOptions) => RenderUrl;
    }

    private class TestReport : IReport
    {
        public object? GetData() => null;
    }

    [UseWKHtmlToPdf(true)]
    private class WkReport : IReport
    {
        public object? GetData() => null;
    }

    private class CustomizingReport : IReport, ICustomizeHtmlToPdf
    {
        public bool Customized { get; private set; }
        public object? GetData() => null;
        public void Customize(IHtmlToPdfOptions options) => Customized = true;
    }

    private class TestRenderer(
        IHtmlToPdfConverter converter,
        IHtmlReportRenderUrlBuilder builder,
        IWKHtmlToPdfConverter? wkConverter = null)
        : HtmlReportPdfRenderer(converter, builder, wkConverter)
    {
        public IHtmlToPdfConverter GetConverterForExposed(IReport report, ReportRenderOptions options) =>
            GetConverterFor(report, options);
        public IHtmlToPdfOptions GetConverterOptionsExposed(IReport report, ReportRenderOptions options,
            out HtmlReportRenderUrl renderUrl) => GetConverterOptions(report, options, out renderUrl);
    }

    [Fact]
    public void Constructor_Throws_For_Null_Arguments()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new HtmlReportPdfRenderer(null!, new FakeRenderUrlBuilder()));
        Assert.Throws<ArgumentNullException>(() =>
            new HtmlReportPdfRenderer(new FakeConverter(), null!));
    }

    [Fact]
    public void Render_Throws_For_Null_RenderOptions()
    {
        var renderer = new HtmlReportPdfRenderer(new FakeConverter(), new FakeRenderUrlBuilder());
        Assert.Throws<ArgumentNullException>(() => renderer.Render(new TestReport(), null!));
    }

    [Fact]
    public void Render_Converts_And_Forwards_Cookies()
    {
        var converter = new FakeConverter();
        var builder = new FakeRenderUrlBuilder();
        builder.RenderUrl.CookiesToForward.Add(new Cookie("auth", "value"));
        var renderer = new HtmlReportPdfRenderer(converter, builder);

        var bytes = renderer.Render(new TestReport(), new ReportRenderOptions());

        Assert.Equal([1, 2, 3], bytes);
        Assert.Equal("https://site/report", converter.Options!.Url);
        Assert.True(converter.Options.DisableLocalFileAccess);
        Assert.Contains("/tmp/a", converter.Options.AllowedLocalPaths);
        Assert.Equal("value", converter.Options.Cookies["auth"]);
        Assert.True(builder.RenderUrl.Disposed);
    }

    [Fact]
    public void GetConverterFor_Prefers_WkHtml_When_Attribute_Present()
    {
        var wkConverter = new FakeWkConverter();
        var renderer = new TestRenderer(new FakeConverter(), new FakeRenderUrlBuilder(), wkConverter);

        Assert.Same(wkConverter, renderer.GetConverterForExposed(new WkReport(), new ReportRenderOptions()));
        Assert.NotSame(wkConverter, renderer.GetConverterForExposed(new TestReport(), new ReportRenderOptions()));
    }

    [Fact]
    public void GetConverterOptions_Customizes_Report()
    {
        var report = new CustomizingReport();
        var renderer = new TestRenderer(new FakeConverter(), new FakeRenderUrlBuilder());

        var options = renderer.GetConverterOptionsExposed(report, new ReportRenderOptions(), out _);

        Assert.True(report.Customized);
        Assert.NotNull(options);
    }

    [Fact]
    public void GetConverterOptions_Disposes_RenderUrl_On_Error()
    {
        var builder = new FakeRenderUrlBuilder();
        builder.RenderUrl.ThrowOnTemporaryFolders = true;
        var renderer = new TestRenderer(new FakeConverter(), builder);

        Assert.Throws<InvalidOperationException>(() =>
            renderer.GetConverterOptionsExposed(new TestReport(), new ReportRenderOptions(), out _));
        Assert.True(builder.RenderUrl.Disposed);
    }
}
