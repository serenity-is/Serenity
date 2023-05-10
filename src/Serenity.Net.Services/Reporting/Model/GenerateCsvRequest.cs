namespace Serenity.Reporting;

/// <summary>
/// Request model for a CSV exporter.
/// </summary>
public class GenerateCsvRequest : ServiceRequest
{
    /// <summary>
    /// List of column captions, e.g. column names in CSV
    /// </summary>
    public List<string> Captions { get; set; }

    /// <summary>
    /// List of data containing column values in caption
    /// order.
    /// </summary>
    public List<string[]> Data { get; set; }

    /// <summary>
    /// The download name for the exported file.
    /// </summary>
    public string DownloadName { get; set; }
}