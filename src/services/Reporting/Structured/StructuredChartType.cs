namespace Serenity.Reporting;

/// <summary>
/// The type of a <see cref="StructuredChart"/>.
/// </summary>
public enum StructuredChartType
{
    /// <summary>Vertical bars for each label, e.g. a period, with a bar for each series side by side.</summary>
    Columns = 0,
    /// <summary>Horizontal bars for each label, e.g. a ranking.</summary>
    Bars = 1,
    /// <summary>A line for each series, e.g. a trend.</summary>
    Line = 2,
    /// <summary>The share of each label in the total of the first series.</summary>
    Donut = 3,
    /// <summary>A tile map whose labels are country or state codes.</summary>
    Map = 4,
    /// <summary>A single bar with a segment for each label, e.g. for age brackets.</summary>
    Segments = 5
}
