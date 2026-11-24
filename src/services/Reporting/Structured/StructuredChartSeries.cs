namespace Serenity.Reporting;

/// <summary>
/// A series of a <see cref="StructuredChart"/>, with a value for each of its labels.
/// </summary>
public class StructuredChartSeries
{
    /// <summary>
    /// The name of the series, shown in the legend.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The values of the series, in the order of the labels of the chart.
    /// </summary>
    public List<double?> Values { get; set; } = [];
}
