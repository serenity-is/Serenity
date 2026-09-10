namespace Serenity.Reporting;

public class ReportingAttributesTests
{
    [Fact]
    public void ReportDesignAttribute_Throws_For_Null()
    {
        Assert.Throws<ArgumentNullException>(() => new ReportDesignAttribute(null));
    }

    [Fact]
    public void ReportDesignAttribute_Stores_Design()
    {
        Assert.Equal("~/Reports/Test.cshtml", new ReportDesignAttribute("~/Reports/Test.cshtml").Design);
    }

    [Fact]
    public void UseWKHtmlToPdfAttribute_Defaults_To_True()
    {
        Assert.True(new UseWKHtmlToPdfAttribute().Value);
    }

    [Fact]
    public void UseWKHtmlToPdfAttribute_Stores_Value()
    {
        Assert.False(new UseWKHtmlToPdfAttribute(false).Value);
    }

    [Fact]
    public void UseChromeHtmlToPdfAttribute_Uses_False()
    {
#pragma warning disable CS0618
        Assert.False(new UseChromeHtmlToPdfAttribute().Value);
#pragma warning restore CS0618
    }

    [Fact]
    public void CellDecoratorAttribute_Stores_Type()
    {
        var attr = new CellDecoratorAttribute(typeof(EnumDecorator));
        Assert.Equal(typeof(EnumDecorator), attr.DecoratorType);
    }

    [Fact]
    public void ReportRenderResult_Initializes_ViewData()
    {
        var result = new ReportRenderResult();
        Assert.NotNull(result.ViewData);
        Assert.Empty(result.ViewData);
    }

    [Fact]
    public void ReportRenderResult_Properties_Are_Settable()
    {
        var result = new ReportRenderResult
        {
            ContentBytes = [1, 2, 3],
            FileName = "file",
            FileExtension = "pdf",
            MimeType = "application/pdf",
            ViewName = "view",
            Model = "model",
            RedirectUri = "url"
        };

        Assert.Equal([1, 2, 3], result.ContentBytes);
        Assert.Equal("file", result.FileName);
        Assert.Equal("pdf", result.FileExtension);
        Assert.Equal("application/pdf", result.MimeType);
        Assert.Equal("view", result.ViewName);
        Assert.Equal("model", result.Model);
        Assert.Equal("url", result.RedirectUri);
    }

    [Fact]
    public void ReportRenderOptions_Properties_Are_Settable()
    {
        var options = new ReportRenderOptions
        {
            PreviewMode = true,
            ReportKey = "key",
            ReportParams = "params",
            ExportFormat = "xlsx"
        };

        Assert.True(options.PreviewMode);
        Assert.Equal("key", options.ReportKey);
        Assert.Equal("params", options.ReportParams);
        Assert.Equal("xlsx", options.ExportFormat);
    }

    [Fact]
    public void ReportColumn_Properties_Are_Settable()
    {
        var decorator = new EnumDecorator(typeof(RetrieveColumnSelection), NullTextLocalizer.Instance);
        var column = new ReportColumn
        {
            Name = "Name",
            Title = "Title",
            Width = 10,
            DataType = typeof(int),
            Format = "0.00",
            WrapText = true,
            Decorator = decorator
        };

        Assert.Equal("Name", column.Name);
        Assert.Equal("Title", column.Title);
        Assert.Equal(10, column.Width);
        Assert.Equal(typeof(int), column.DataType);
        Assert.Equal("0.00", column.Format);
        Assert.True(column.WrapText);
        Assert.Same(decorator, column.Decorator);
    }

    [Fact]
    public void ReportRetrieveModels_Properties_Are_Settable()
    {
        var request = new ReportRetrieveRequest { ReportKey = "key" };
        Assert.Equal("key", request.ReportKey);

        var response = new ReportRetrieveResponse
        {
            ReportKey = "key",
            Title = "title",
            Properties = [],
            InitialSettings = "settings",
            IsDataOnlyReport = true,
            IsExternalReport = true
        };

        Assert.Equal("key", response.ReportKey);
        Assert.Equal("title", response.Title);
        Assert.NotNull(response.Properties);
        Assert.Equal("settings", response.InitialSettings);
        Assert.True(response.IsDataOnlyReport);
        Assert.True(response.IsExternalReport);
    }

    [Fact]
    public void GenerateCsvRequest_Properties_Are_Settable()
    {
        var request = new GenerateCsvRequest
        {
            Captions = ["A"],
            Data = [["1"]],
            DownloadName = "file"
        };

        Assert.Single(request.Captions);
        Assert.Single(request.Data);
        Assert.Equal("file", request.DownloadName);
    }

    [Fact]
    public void GenerateExcelFileRequest_Properties_Are_Settable()
    {
        var request = new GenerateExcelFileRequest
        {
            Captions = ["A"],
            Data = [[1]],
            DownloadName = "file"
        };

        Assert.Single(request.Captions);
        Assert.Single(request.Data);
        Assert.Equal("file", request.DownloadName);
    }
}
