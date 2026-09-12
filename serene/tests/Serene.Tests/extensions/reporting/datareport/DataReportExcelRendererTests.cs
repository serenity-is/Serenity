using System.Collections;
using ClosedXML.Excel;

namespace Serenity.Reporting;

public class DataReportExcelRendererTests
{
    private class TestReport : IDataOnlyReport
    {
        public List<ReportColumn> Columns { get; set; } = [new ReportColumn { Name = "Name" }];
        public object? Data { get; set; } = new { Name = "A" };

        public List<ReportColumn> GetColumnList() => Columns;
        public object? GetData() => Data;
    }

    [Fact]
    public void Render_Throws_For_Null_Report()
    {
        Assert.Throws<ArgumentNullException>(() => new DataReportExcelRenderer().Render(null!));
    }

    [Fact]
    public void Render_Returns_Excel_Bytes_For_Enumerable_Data()
    {
        var bytes = new DataReportExcelRenderer().Render(new TestReport
        {
            Data = new List<object> { new { Name = "A" }, new { Name = "B" } }
        });

        Assert.NotEmpty(bytes);
    }

    [Fact]
    public void Render_Returns_Excel_Bytes_For_Single_Data()
    {
        var bytes = new DataReportExcelRenderer().Render(new TestReport());
        Assert.NotEmpty(bytes);
    }
}
