namespace Serenity.Reporting;

/// <summary>
/// A key figure of a <see cref="StructuredReportResult"/>, e.g. the revenue in the period of a report.
/// </summary>
public class StructuredKpi
{
    /// <summary>
    /// The label of the figure.
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// The value of the figure, which is formatted by its <see cref="Kind"/>.
    /// </summary>
    public object? Value { get; set; }

    /// <summary>
    /// The kind of the value, e.g. money or percent.
    /// </summary>
    public ReportValueKind Kind { get; set; }

    /// <summary>
    /// A short note shown under the value, e.g. a comparison with a prior period.
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// The tone of the figure, i.e. "ok", "warn", "danger", "info" or "muted", if it should stand out.
    /// </summary>
    public string? Tone { get; set; }
}
