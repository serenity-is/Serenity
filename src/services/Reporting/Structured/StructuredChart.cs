namespace Serenity.Reporting;

/// <summary>
/// A chart of a <see cref="StructuredReportResult"/>, which describes what to show and leaves how to draw it to the client.
/// </summary>
public class StructuredChart
{
    /// <summary>
    /// The type of the chart.
    /// </summary>
    public StructuredChartType Type { get; set; }

    /// <summary>
    /// The title of the chart.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// A short line under the title.
    /// </summary>
    public string? Subtitle { get; set; }

    /// <summary>
    /// The labels of the chart, e.g. its periods, names, or country codes for a map.
    /// </summary>
    public List<string> Labels { get; set; } = [];

    /// <summary>
    /// The series of the chart, each with a value for each label.
    /// </summary>
    public List<StructuredChartSeries> Series { get; set; } = [];

    /// <summary>
    /// The kind of the values, e.g. money, which is used to format them.
    /// </summary>
    public ReportValueKind Kind { get; set; }
}
