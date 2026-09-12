using Serenity.Reporting;
#pragma warning disable CS0618

namespace Serenity.Extensions.Repositories;

public class ReportRepositoryTests
{
    private class TestReport : IReport
    {
        public object? GetData() => null;
    }

    private class FakeRegistry(ReportRegistry.Report? report) : IReportRegistry
    {
        public ReportRegistry.Report? GetReport(string reportKey, bool validatePermission = true) => report;

        public IEnumerable<ReportRegistry.Report> GetAvailableReportsInCategory(string categoryKey) =>
            report == null ? [] : [report];

        public bool HasAvailableReportsInCategory(string categoryKey) => report != null;
    }

    [Fact]
    public void Constructor_Throws_For_Null_Registry()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ReportRepository(new NullRequestContext(), null!));
    }

    [Fact]
    public void GetReportTree_Returns_Tree()
    {
        var registry = new FakeRegistry(new ReportRegistry.Report(typeof(TestReport), NullTextLocalizer.Instance));
        var repository = new ReportRepository(new NullRequestContext(), registry);

        var tree = repository.GetReportTree("");
        Assert.Single(tree.Root.Reports);
    }

    [Fact]
    public void Retrieve_Throws_For_Null_Arguments()
    {
        var repository = new ReportRepository(new NullRequestContext(), new FakeRegistry(null));
        Assert.Throws<ArgumentNullException>(() =>
            repository.Retrieve(null!, new ServiceCollection().BuildServiceProvider(), new MockPropertyItemProvider()));
        Assert.Throws<ArgumentNullException>(() =>
            repository.Retrieve(new ReportRetrieveRequest(), new ServiceCollection().BuildServiceProvider(), null!));
        Assert.Throws<ArgumentNullException>(() =>
            repository.Retrieve(new ReportRetrieveRequest { ReportKey = null }, new ServiceCollection().BuildServiceProvider(), new MockPropertyItemProvider()));
    }

    [Fact]
    public void Retrieve_Returns_Report_Info()
    {
        var registry = new FakeRegistry(new ReportRegistry.Report(typeof(TestReport), NullTextLocalizer.Instance));
        var repository = new ReportRepository(new NullRequestContext(), registry);

        var response = repository.Retrieve(new ReportRetrieveRequest { ReportKey = "Test" },
            new ServiceCollection().BuildServiceProvider(), new MockPropertyItemProvider());

        Assert.NotNull(response.InitialSettings);
        Assert.IsType<TestReport>(response.InitialSettings);
    }

    [Fact]
    public void Retrieve_Throws_When_Not_Found()
    {
        var repository = new ReportRepository(new NullRequestContext(), new FakeRegistry(null));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            repository.Retrieve(new ReportRetrieveRequest { ReportKey = "Unknown" },
                new ServiceCollection().BuildServiceProvider(), new MockPropertyItemProvider()));
    }
}

