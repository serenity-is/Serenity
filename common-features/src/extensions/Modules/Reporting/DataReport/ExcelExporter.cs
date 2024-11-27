using System.Collections;

namespace Serenity.Reporting;

public class ExcelExporter(IDataReportExcelRenderer renderer, IServiceProvider serviceProvider) : IExcelExporter
{
    private readonly IDataReportExcelRenderer renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
    private readonly IServiceProvider serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    public byte[] Export(IEnumerable data, IEnumerable<ReportColumn> columns)
    {
        var report = new TabularDataReport(data, columns);
        return renderer.Render(report);
    }

    public byte[] Export(IEnumerable data, Type columnsType)
    {
        var report = new TabularDataReport(data, columnsType, serviceProvider);
        return renderer.Render(report);
    }

    public byte[] Export(IEnumerable data, Type columnsType, IEnumerable<string> exportColumns)
    {
        var report = new TabularDataReport(data, columnsType, exportColumns, serviceProvider);
        return renderer.Render(report);
    }
}