namespace Serenity.Reporting;

/// <summary>
/// Abstraction for the a report factory that creates a report by its key
/// and populates it with the JSON options passed
/// </summary>
public interface IReportFactory
{
    /// <summary>
    /// Creates a report for the given report key
    /// </summary>
    /// <param name="reportKey">Report key</param>
    /// <param name="reportParams">Optional JSON options to set in the report</param>
    /// <param name="validatePermission">True if the permission for the report should be validated</param>
    /// <returns></returns>
    public IReport Create(string reportKey, string reportParams, bool validatePermission = true);

    /// <summary>
    /// Sets options for an externally created report object
    /// </summary>
    /// <param name="report">Report</param>
    /// <param name="reportParams">JSON serialized options</param>
    public void SetParams(IReport report, string reportParams);
}