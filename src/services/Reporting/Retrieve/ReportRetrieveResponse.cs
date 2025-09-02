namespace Serenity.Reporting;

/// <summary>
/// Response model for a report retrieve request.
/// </summary>
public class ReportRetrieveResponse : ServiceResponse
{
    /// <summary>
    /// The report key.
    /// </summary>
    public string ReportKey { get; set; }

    /// <summary>
    /// The title of the report.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// List of parameters as property items to 
    /// show in report execution form.
    /// </summary>
    public List<PropertyItem> Properties { get; set; }

    /// <summary>
    /// The initial settings for report parameters.
    /// </summary>
    public object InitialSettings { get; set; }

    /// <summary>
    /// True if the report is an <see cref="IDataOnlyReport"/>
    /// </summary>
    public bool IsDataOnlyReport { get; set; }

    /// <summary>
    /// True if the report is an <see cref="IExternalReport"/>
    /// </summary>
    public bool IsExternalReport { get; set; }
}