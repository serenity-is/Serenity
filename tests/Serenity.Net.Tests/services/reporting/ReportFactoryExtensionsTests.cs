namespace Serenity.Reporting;

public class ReportFactoryExtensionsTests
{
    [Report("My.Report.Key")]
    private class MyReport : IReport
    {
        public object? GetData() => null;
    }

    private class MockReportFactory(IReport? report) : IReportFactory
    {
        public string? Key { get; private set; }
        public string? Params { get; private set; }
        public bool ValidatePermission { get; private set; }

        public IReport Create(string reportKey, string? reportParams, bool validatePermission = true)
        {
            Key = reportKey;
            Params = reportParams;
            ValidatePermission = validatePermission;
            return report;
        }

        public void SetParams(IReport report, string reportParams)
        {
        }
    }

    [Fact]
    public void Create_Looks_Up_Report_Key_And_Validates_Permission()
    {
        var expected = new MyReport();
        var factory = new MockReportFactory(expected);

        var report = factory.Create<MyReport>();

        Assert.Same(expected, report);
        Assert.Equal("My.Report.Key", factory.Key);
        Assert.Null(factory.Params);
        Assert.True(factory.ValidatePermission);
    }

    [Fact]
    public void Create_Invokes_SetParams_Callback()
    {
        var expected = new MyReport();
        var factory = new MockReportFactory(expected);

        MyReport? captured = null;
        var report = factory.Create<MyReport>(r => captured = r);

        Assert.Same(expected, report);
        Assert.Same(expected, captured);
    }

    [Fact]
    public void Create_Passes_ValidatePermission_False()
    {
        var factory = new MockReportFactory(new MyReport());

        factory.Create<MyReport>(validatePermission: false);

        Assert.False(factory.ValidatePermission);
    }
}

public class BaseReportTests
{
    private class TestReport : BaseReport
    {
        public override object? GetData() => "data";
    }

    [Fact]
    public void GetAdditionalData_Returns_Null_By_Default()
    {
        var report = new TestReport();
        Assert.Null(report.GetAdditionalData());
    }

    [Fact]
    public void GetData_Is_Implemented_By_Derived()
    {
        Assert.Equal("data", new TestReport().GetData());
    }
}
