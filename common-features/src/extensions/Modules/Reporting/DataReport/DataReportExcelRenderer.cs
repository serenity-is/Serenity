using System.Collections;

namespace Serenity.Reporting;

/// <summary>
/// Default implementation of <see cref="IDataReportExcelRenderer"/> that renders
/// a data report to an Excel package.
/// </summary>
public class DataReportExcelRenderer : IDataReportExcelRenderer
{
    /// <summary>
    /// Renders the specified data report to Excel bytes.
    /// </summary>
    /// <param name="report">The data report.</param>
    /// <returns>The generated Excel file bytes.</returns>
    public byte[] Render(IDataOnlyReport report)
    {
        ArgumentNullException.ThrowIfNull(report);

        var columns = report.GetColumnList();

        var data = new List<object>();
        var input = report.GetData();
        var list = (input as IEnumerable) ?? new List<object> { input };
        foreach (var item in list)
            data.Add(item);

        return ExcelReportGenerator.GeneratePackageBytes(columns, data);
    }
}