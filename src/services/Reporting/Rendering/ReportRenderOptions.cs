namespace Serenity.Reporting;

/// <summary>
/// Render request a report
/// </summary>
public class ReportRenderOptions
{
    /// <summary>
    /// Gets / set if rendering in preview mode, only useful for HTML output
    /// </summary>
    public bool PreviewMode { get; set; }

    /// <summary>The report key, if it is not specified in report type as an attribute.
    /// Will be ignored if report will be rendered directly, e.g. not via a callback.</summary>
    public string ReportKey { get; set; }

    /// <summary>The set of report params usually a serialized JSON object 
    /// to be passed to the URL callback. These params should already be applied to the passed
    /// report instance. Will be ignored if the report is rendered directly, e.g. without a callback.</summary>
    public string ReportParams { get; set; }

    /// <summary>
    /// The requested export format if specified, like "xlsx", "html", "html", "pdf" etc. This is only
    /// useful for cases where a report can be rendered to multiple types.
    /// </summary>
    public string ExportFormat { get; set; }
}