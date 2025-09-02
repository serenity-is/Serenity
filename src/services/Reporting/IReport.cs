
namespace Serenity.Reporting;

/// <summary>
/// The basic interface for all reports
/// </summary>
public interface IReport
{
    /// <summary>
    /// Returns the data for the report
    /// </summary>
    object GetData();
}