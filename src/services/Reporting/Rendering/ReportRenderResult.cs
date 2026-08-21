namespace Serenity.Reporting;

/// <summary>
/// Render result for a report.
/// </summary>
public class ReportRenderResult 
{
    /// <summary>
    /// Gets or sets the file content bytes.
    /// </summary>
    public byte[] ContentBytes { get; set; }

    /// <summary>
    /// Gets or sets the suggested file name (without extension).
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// Gets or sets the file extension.
    /// </summary>
    public string FileExtension { get; set; }

    /// <summary>
    /// Gets or sets the MIME type, if available.
    /// </summary>
    public string MimeType { get; set; }

    /// <summary>
    /// Gets or sets a view name, only returned in preview mode for HTML.
    /// </summary>
    public string ViewName { get; set; }

    /// <summary>
    /// Gets or sets the view model, only returned in preview mode for HTML.
    /// </summary>
    public object Model { get; set; }

    /// <summary>
    /// Gets a list of view data to pass to the view data dictionary,
    /// only returned in preview mode for HTML.
    /// </summary>
    public IDictionary<string, object> ViewData { get; private set; } = new Dictionary<string, object>();

    /// <summary>
    /// Gets or sets a redirect URI, only returned for external reports.
    /// </summary>
    public string RedirectUri { get; set; }
}