using System.Collections;

namespace Serenity.Reporting;

/// <summary>
/// Interface for Excel exporters.
/// </summary>
public interface IExcelExporter
{
    /// <summary>
    /// Exports the specified data to Excel format using the given report columns.
    /// </summary>
    /// <param name="data">The data to export.</param>
    /// <param name="columns">The columns to export.</param>
    /// <returns>The generated Excel file content.</returns>
    byte[] Export(IEnumerable data, IEnumerable<ReportColumn> columns);

    /// <summary>
    /// Exports the specified data to Excel format, deriving the report columns from a columns type.
    /// </summary>
    /// <param name="data">The data to export.</param>
    /// <param name="columnsType">The columns type that will be used
    /// to determine report columns to export.</param>
    /// <returns>The generated Excel file content.</returns>
    byte[] Export(IEnumerable data, Type columnsType);

    /// <summary>
    /// Exports the specified data to Excel format, deriving the report columns from a columns type
    /// and restricting the output to the given column names.
    /// </summary>
    /// <param name="data">The data to export.</param>
    /// <param name="columnsType">The columns type that will be used
    /// to determine available report columns to export.</param>
    /// <param name="exportColumns">Determines the names and order of
    /// columns to be exported.</param>
    /// <returns>The generated Excel file content.</returns>
    byte[] Export(IEnumerable data, Type columnsType, IEnumerable<string> exportColumns);
}