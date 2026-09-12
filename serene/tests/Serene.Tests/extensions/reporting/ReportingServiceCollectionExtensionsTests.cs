using Serenity.Reporting;
namespace Serenity.Extensions.DependencyInjection;

public class ReportingServiceCollectionExtensionsTests
{
    [Fact]
    public void AddExcelExporter_Throws_For_Null()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ReportingServiceCollectionExtensions.AddExcelExporter(null!));
    }

    [Fact]
    public void AddExcelExporter_Registers_Services()
    {
        var services = new ServiceCollection();
        services.AddExcelExporter();

        Assert.Contains(services, x => x.ServiceType == typeof(IDataReportExcelRenderer));
        Assert.Contains(services, x => x.ServiceType == typeof(IExcelExporter));
    }

    [Fact]
    public void AddHtmlToPdf_Throws_For_Null()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ReportingServiceCollectionExtensions.AddHtmlToPdf(null!));
    }

    [Fact]
    public void AddHtmlToPdf_Registers_Services()
    {
        var services = new ServiceCollection();
        services.AddHtmlToPdf();

        Assert.Contains(services, x => x.ServiceType == typeof(ISiteAbsoluteUrl));
        Assert.Contains(services, x => x.ServiceType == typeof(IHtmlReportCallbackUrlBuilder));
        Assert.Contains(services, x => x.ServiceType == typeof(IHtmlReportRenderUrlBuilder));
        Assert.Contains(services, x => x.ServiceType == typeof(IReportCallbackInterceptor));
        Assert.Contains(services, x => x.ServiceType == typeof(IWKHtmlToPdfConverter));
        Assert.Contains(services, x => x.ServiceType == typeof(IHtmlToPdfConverter));
        Assert.Contains(services, x => x.ServiceType == typeof(IHtmlReportPdfRenderer));
    }

    [Fact]
    public void AddReporting_Registers_All_Services()
    {
        var services = new ServiceCollection();
        services.AddReporting();

        Assert.Contains(services, x => x.ServiceType == typeof(IReportRegistry));
        Assert.Contains(services, x => x.ServiceType == typeof(IReportRetrieveHandler));
        Assert.Contains(services, x => x.ServiceType == typeof(IReportTreeFactory));
        Assert.Contains(services, x => x.ServiceType == typeof(IReportFactory));
        Assert.Contains(services, x => x.ServiceType == typeof(IReportRenderer));
        Assert.Contains(services, x => x.ServiceType == typeof(IExcelExporter));
        Assert.Contains(services, x => x.ServiceType == typeof(IHtmlReportPdfRenderer));
    }
}

