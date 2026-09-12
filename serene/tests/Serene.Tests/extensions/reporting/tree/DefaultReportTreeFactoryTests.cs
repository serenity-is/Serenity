namespace Serenity.Reporting;

public class DefaultReportTreeFactoryTests
{
    private class TestReport : IReport
    {
        public object? GetData() => null;
    }

    private class FakeRegistry : IReportRegistry
    {
        public List<ReportRegistry.Report> Reports { get; } = [];

        public ReportRegistry.Report? GetReport(string reportKey, bool validatePermission = true) => null;

        public IEnumerable<ReportRegistry.Report> GetAvailableReportsInCategory(string categoryKey) => Reports;

        public bool HasAvailableReportsInCategory(string categoryKey) => Reports.Count > 0;
    }

    [Fact]
    public void Constructor_Throws_For_Null_Arguments()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new DefaultReportTreeFactory(null!, NullTextLocalizer.Instance));
        Assert.Throws<ArgumentNullException>(() =>
            new DefaultReportTreeFactory(new FakeRegistry(), null!));
    }

    [Fact]
    public void BuildReportTree_Returns_Tree_From_Registry()
    {
        var registry = new FakeRegistry();
        registry.Reports.Add(new ReportRegistry.Report(typeof(TestReport), NullTextLocalizer.Instance));

        var factory = new DefaultReportTreeFactory(registry, NullTextLocalizer.Instance);
        var tree = factory.BuildReportTree("");

        Assert.Single(tree.Root.Reports);
    }
}
