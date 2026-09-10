namespace Serenity.Reporting;

public class HtmlToPdfOptionsTests
{
    [Fact]
    public void Defaults_Are_Set()
    {
        var options = new HtmlToPdfOptions();

        Assert.Empty(options.AdditionalUrls);
        Assert.Empty(options.Cookies);
        Assert.Empty(options.CustomArgs);
        Assert.Empty(options.FooterHeaderReplace);
        Assert.Empty(options.AllowedLocalPaths);
        Assert.Equal(300, options.TimeoutSeconds);
        Assert.True(options.UsePrintMediaType);
        Assert.True(options.PrintBackground);
        Assert.Equal("A4", options.PageSize);
        Assert.Equal("10mm", options.MarginLeft);
        Assert.Equal("10mm", options.MarginRight);
        Assert.Equal("10mm", options.MarginBottom);
        Assert.Equal("10mm", options.MarginTop);
        Assert.True(options.DisableLocalFileAccess);
    }

    [Fact]
    public void MarginsAll_Sets_All_Margins()
    {
        var options = new HtmlToPdfOptions
        {
            MarginsAll = "5mm"
        };

        Assert.Equal("5mm", options.MarginLeft);
        Assert.Equal("5mm", options.MarginRight);
        Assert.Equal("5mm", options.MarginBottom);
        Assert.Equal("5mm", options.MarginTop);
    }

    [Fact]
    public void Properties_Are_Settable()
    {
        var options = new HtmlToPdfOptions
        {
            Url = "http://localhost",
            PageSize = "Letter",
            PageHeight = "100",
            PageWidth = "200",
            SmartShrinking = true,
            Dpi = 96,
            Landscape = true,
            Zoom = "1.5",
            HeaderHtmlUrl = "header",
            FooterHtmlUrl = "footer",
            DisableLocalFileAccess = false,
            EditLaunchOptions = _ => { },
            EditPdfOptions = _ => { }
        };

        Assert.NotNull(options.EditLaunchOptions);
        Assert.NotNull(options.EditPdfOptions);

        Assert.Equal("http://localhost", options.Url);
        Assert.Equal("Letter", options.PageSize);
        Assert.Equal("100", options.PageHeight);
        Assert.Equal("200", options.PageWidth);
        Assert.True(options.SmartShrinking);
        Assert.Equal(96, options.Dpi);
        Assert.True(options.Landscape);
        Assert.Equal("1.5", options.Zoom);
        Assert.Equal("header", options.HeaderHtmlUrl);
        Assert.Equal("footer", options.FooterHtmlUrl);
        Assert.False(options.DisableLocalFileAccess);
    }
}
