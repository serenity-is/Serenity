using System.Collections;

namespace Serenity.Reporting;

/// <summary>
/// Interface for Excel exporters
/// </summary>
public interface IExcelExporter
{
    /// <summary>
    /// Exports the specified data to Excel format
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="columns">The columns to export.</param>
    byte[] Export(IEnumerable data, IEnumerable<ReportColumn> columns);

    /// <summary>
    /// Exports the specified data to Excel format
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="columnsType">The columns type that will be used
    /// to determine report columns to export.</param>
    byte[] Export(IEnumerable data, Type columnsType);

    /// <summary>
    /// Exports the specified data to Excel format
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="columnsType">The columns type that will be used
    /// to determine available report columns to export.</param>
    /// <param name="exportColumns">Determines the names and order of 
    /// columns to be exported</param>
    byte[] Export(IEnumerable data, Type columnsType, IEnumerable<string> exportColumns);
}