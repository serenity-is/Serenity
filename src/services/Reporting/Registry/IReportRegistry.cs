namespace Serenity.Reporting;

/// <summary>
/// Abstraction for the registry which holds and allows querying
/// list of available reports by their keys and categories.
/// </summary>
public interface IReportRegistry
{
    /// <summary>
    /// Gets all available reports in category, for example "Northwind".
    /// This only returns reports that current user has access to, by
    /// checking their permissions if any.
    /// </summary>
    /// <param name="categoryKey">The category key.</param>
    /// <returns>The available reports in that category.</returns>
    IEnumerable<ReportRegistry.Report> GetAvailableReportsInCategory(string categoryKey);

    /// <summary>
    /// Gets a report by its key.
    /// </summary>
    /// <param name="reportKey">Report key</param>
    /// <param name="validatePermission">Should validate the permission for report before returning.</param>
    /// <returns></returns>
    ReportRegistry.Report GetReport(string reportKey, bool validatePermission = true);

    /// <summary>
    /// Returns true if there are reports in the category passed.
    /// </summary>
    /// <param name="categoryKey">The category key.</param>
    bool HasAvailableReportsInCategory(string categoryKey);
}