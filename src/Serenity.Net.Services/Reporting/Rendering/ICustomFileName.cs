
namespace Serenity.Reporting;

/// <summary>
/// Interface to customize the output file name, 
/// e.g. name of the XLSX or PDF file outputted by
/// reports. By default, reports has output file name
/// of format "{ReportKey}_yyyyMMdd_HHmmss.ext".
/// </summary>
public interface ICustomFileName
{
    /// <summary>
    /// Returns the custom output file name.
    /// </summary>
    string GetFileName();
}