using System.Collections;

namespace Serenity.Reporting;

/// <summary>
/// Default implementation of <see cref="IExcelExporter"/> that exports data to Excel.
/// </summary>
public class ExcelExporter(IDataReportExcelRenderer renderer, IServiceProvider serviceProvider) : IExcelExporter
{
    private readonly IDataReportExcelRenderer renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
    private readonly IServiceProvider serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    /// <summary>
    /// Exports the specified data with the given columns to Excel bytes.
    /// </summary>
    /// <param name="data">The data to export.</param>
    /// <param name="columns">The report columns.</param>
    /// <returns>The generated Excel file bytes.</returns>
    public byte[] Export(IEnumerable data, IEnumerable<ReportColumn> columns)
    {
        var report = new TabularDataReport(data, columns);
        return renderer.Render(report);
    }

    /// <summary>
    /// Exports the specified data using the columns defined by the given type.
    /// </summary>
    /// <param name="data">The data to export.</param>
    /// <param name="columnsType">The type defining the report columns.</param>
    /// <returns>The generated Excel file bytes.</returns>
    public byte[] Export(IEnumerable data, Type columnsType)
    {
        var report = new TabularDataReport(data, columnsType, serviceProvider);
        return renderer.Render(report);
    }

    /// <summary>
    /// Exports the specified data using the columns defined by the given type, limited to the specified columns.
    /// </summary>
    /// <param name="data">The data to export.</param>
    /// <param name="columnsType">The type defining the report columns.</param>
    /// <param name="exportColumns">The subset of columns to export.</param>
    /// <returns>The generated Excel file bytes.</returns>
    public byte[] Export(IEnumerable data, Type columnsType, IEnumerable<string> exportColumns)
    {
        var report = new TabularDataReport(data, columnsType, exportColumns, serviceProvider);
        return renderer.Render(report);
    }
}