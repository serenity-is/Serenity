namespace Serenity.Reporting;

/// <summary>
/// Render result for a report
/// </summary>
public class ReportRenderResult 
{
    /// <summary>
    /// File content bytes
    /// </summary>
    public byte[] ContentBytes { get; set; }

    /// <summary>
    /// File extension
    /// </summary>
    public string FileExtension { get; set; }

    /// <summary>
    /// Mime type if available
    /// </summary>
    public string MimeType { get; set; }

    /// <summary>
    /// A view name, only returned in preview mode for HTML
    /// </summary>
    public string ViewName { get; set; }

    /// <summary>
    /// View model, only returned in preview mode for HTML
    /// </summary>
    public object Model { get; set; }

    /// <summary>
    /// A list of view data to pass to the view data dictionary, only returned in preview mode for HTML
    /// </summary>
    public IDictionary<string, object> ViewData { get; private set; } = new Dictionary<string, object>();

    /// <summary>
    /// A redirect URI, only returned for external reports
    /// </summary>
    public string RedirectUri { get; set; }
}