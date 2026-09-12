using System.Collections;

namespace Serenity.Reporting;

public class ExcelExporterTests
{
    private class FakeRenderer : IDataReportExcelRenderer
    {
        public IDataOnlyReport? Report { get; private set; }
        public byte[] Bytes { get; set; } = [1, 2, 3];

        public byte[] Render(IDataOnlyReport report)
        {
            Report = report;
            return Bytes;
        }
    }

    [Fact]
    public void Constructor_Throws_For_Null_Arguments()
    {
        var provider = new ServiceCollection().BuildServiceProvider();
        Assert.Throws<ArgumentNullException>(() => new ExcelExporter(null!, provider));
        Assert.Throws<ArgumentNullException>(() => new ExcelExporter(new FakeRenderer(), null!));
    }

    [Fact]
    public void Export_With_Columns_Uses_Renderer()
    {
        var renderer = new FakeRenderer();
        var exporter = new ExcelExporter(renderer, new ServiceCollection().BuildServiceProvider());

        var bytes = exporter.Export(new ArrayList { new { Name = "A" } },
            new List<ReportColumn> { new() { Name = "Name" } });

        Assert.Equal([1, 2, 3], bytes);
        Assert.NotNull(renderer.Report);
    }

    [Fact]
    public void Export_With_ColumnsType_Uses_Renderer()
    {
        var renderer = new FakeRenderer();
        var exporter = new ExcelExporter(renderer, new ServiceCollection().BuildServiceProvider());

        var bytes = exporter.Export(new ArrayList { new { Name = "A" } }, typeof(object));
        Assert.Equal([1, 2, 3], bytes);
        Assert.NotNull(renderer.Report);
    }

    [Fact]
    public void Export_With_ExportColumns_Uses_Renderer()
    {
        var renderer = new FakeRenderer();
        var exporter = new ExcelExporter(renderer, new ServiceCollection().BuildServiceProvider());

        var bytes = exporter.Export(new ArrayList { new { Name = "A" } }, typeof(object), ["Name"]);
        Assert.Equal([1, 2, 3], bytes);
        Assert.NotNull(renderer.Report);
    }
}
