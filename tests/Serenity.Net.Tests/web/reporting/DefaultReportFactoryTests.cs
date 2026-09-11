namespace Serenity.Reporting;

public class DefaultReportFactoryTests
{
    private class TestReport : IReport
    {
        public string? Foo { get; set; }

        public object GetData() => "data";
    }

    private class FakeRegistry(ReportRegistry.Report? report) : IReportRegistry
    {
        public ReportRegistry.Report? GetReport(string reportKey, bool validatePermission = true) => report;

        public IEnumerable<ReportRegistry.Report> GetAvailableReportsInCategory(string categoryKey) => [];

        public bool HasAvailableReportsInCategory(string categoryKey) => false;
    }

    private static DefaultReportFactory Create(string key = "Test")
    {
        var report = key == "Test"
            ? new ReportRegistry.Report(typeof(TestReport), NullTextLocalizer.Instance)
            : null;
        return new DefaultReportFactory(new FakeRegistry(report),
            new ServiceCollection().BuildServiceProvider());
    }

    [Fact]
    public void Constructor_Throws_For_Null_Arguments()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new DefaultReportFactory(null!, new ServiceCollection().BuildServiceProvider()));
        Assert.Throws<ArgumentNullException>(() =>
            new DefaultReportFactory(new FakeRegistry(null), null!));
    }

    [Fact]
    public void Create_Throws_When_Report_Not_Found()
    {
        var factory = Create("Unknown");
        Assert.Throws<ArgumentOutOfRangeException>(() => factory.Create("Unknown", null, true));
    }

    [Fact]
    public void Create_Returns_Report_Instance()
    {
        var factory = Create();

        var report = Assert.IsType<TestReport>(factory.Create("Test", null, true));

        Assert.Null(report.Foo);
    }

    [Fact]
    public void Create_Applies_Report_Options()
    {
        var factory = Create();

        var report = Assert.IsType<TestReport>(factory.Create("Test", """{"Foo":"bar"}""", true));

        Assert.Equal("bar", report.Foo);
    }

    [Fact]
    public void SetParams_Throws_For_Empty()
    {
        var factory = Create();
        Assert.Throws<ArgumentNullException>(() => factory.SetParams(new TestReport(), null!));
        Assert.Throws<ArgumentNullException>(() => factory.SetParams(new TestReport(), ""));
    }

    [Fact]
    public void SetParams_Ignores_Whitespace()
    {
        var factory = Create();
        var report = new TestReport();

        factory.SetParams(report, "   ");

        Assert.Null(report.Foo);
    }
}
