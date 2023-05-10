namespace Serenity.Reporting;

/// <summary>
/// Request model to retrieve a report by its key.
/// This only returns the metadata, e.g. parameters of report etc.
/// </summary>
public class ReportRetrieveRequest : ServiceRequest
{
    /// <summary>
    /// The report key to retrieve.
    /// </summary>
    public string ReportKey { get; set; }
}