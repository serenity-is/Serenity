namespace Serenity.Reporting;

public class DefaultReportRetrieveHandlerTests
{
    private class TestReport : IReport
    {
        public object? GetData() => null;
    }

    private class TestDataOnlyReport : IDataOnlyReport
    {
        public object? GetData() => null;
        public List<ReportColumn> GetColumnList() => [];
    }

    private class FakeRegistry(ReportRegistry.Report? report) : IReportRegistry
    {
        public ReportRegistry.Report? GetReport(string reportKey, bool validatePermission = true) => report;

        public IEnumerable<ReportRegistry.Report> GetAvailableReportsInCategory(string categoryKey) => [];

        public bool HasAvailableReportsInCategory(string categoryKey) => false;
    }

    private static DefaultReportRetrieveHandler Create(Type reportType)
    {
        var report = new ReportRegistry.Report(reportType, NullTextLocalizer.Instance);
        return new DefaultReportRetrieveHandler(new MockPropertyItemProvider(),
            new FakeRegistry(report), new ServiceCollection().BuildServiceProvider());
    }

    [Fact]
    public void Constructor_Throws_For_Null_Arguments()
    {
        var registry = new FakeRegistry(null);
        var provider = new ServiceCollection().BuildServiceProvider();
        Assert.Throws<ArgumentNullException>(() =>
            new DefaultReportRetrieveHandler(null!, registry, provider));
        Assert.Throws<ArgumentNullException>(() =>
            new DefaultReportRetrieveHandler(new MockPropertyItemProvider(), null!, provider));
        Assert.Throws<ArgumentNullException>(() =>
            new DefaultReportRetrieveHandler(new MockPropertyItemProvider(), registry, null!));
    }

    [Fact]
    public void Retrieve_Throws_For_Null_Or_Empty_Request()
    {
        var handler = Create(typeof(TestReport));
        Assert.Throws<ArgumentNullException>(() => handler.Retrieve(null!));
        Assert.Throws<ArgumentException>(() => handler.Retrieve(new ReportRetrieveRequest { ReportKey = "" }));
    }

    [Fact]
    public void Retrieve_Throws_When_Report_Not_Found()
    {
        var handler = new DefaultReportRetrieveHandler(new MockPropertyItemProvider(),
            new FakeRegistry(null), new ServiceCollection().BuildServiceProvider());
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            handler.Retrieve(new ReportRetrieveRequest { ReportKey = "Unknown" }));
    }

    [Fact]
    public void Retrieve_Returns_Report_Info()
    {
        var handler = Create(typeof(TestReport));
        var response = handler.Retrieve(new ReportRetrieveRequest { ReportKey = "Test" });

        Assert.NotNull(response.InitialSettings);
        Assert.IsType<TestReport>(response.InitialSettings);
        Assert.False(response.IsDataOnlyReport);
        Assert.False(response.IsExternalReport);
    }

    [Fact]
    public void Retrieve_Detects_DataOnly_Report()
    {
        var handler = Create(typeof(TestDataOnlyReport));
        var response = handler.Retrieve(new ReportRetrieveRequest { ReportKey = "Test" });
        Assert.True(response.IsDataOnlyReport);
    }
}
