namespace Serenity.Reporting;

/// <summary>
/// Abstraction for report info retriever
/// </summary>
public interface IReportRetrieveHandler
{
    /// <summary>
    /// Retrieves a report info
    /// </summary>
    /// <param name="request">Request</param>
    public ReportRetrieveResponse Retrieve(ReportRetrieveRequest request);
}
