namespace Serenity.Reporting;

/// <summary>
/// Request model for an Excel exporter.
/// </summary>
public class GenerateExcelFileRequest : ServiceRequest
{
    /// <summary>
    /// List of column captions.
    /// </summary>
    public List<string> Captions { get; set; }

    /// <summary>
    /// List of items, containing an array of field values in each item.
    /// </summary>
    public List<object[]> Data { get; set; }

    /// <summary>
    /// The download name for the exported file.
    /// </summary>
    public string DownloadName { get; set; }
}