namespace Serenity.Reporting;

/// <summary>
/// The kind of a value in a report, which decides how it is formatted when its column has no format of its own.
/// </summary>
public enum ReportValueKind
{
    /// <summary>The kind is decided by the data type of the value.</summary>
    Default = 0,
    /// <summary>A text.</summary>
    Text = 1,
    /// <summary>A whole number.</summary>
    Integer = 2,
    /// <summary>A number with decimals.</summary>
    Number = 3,
    /// <summary>An amount of money.</summary>
    Money = 4,
    /// <summary>A ratio, e.g. 0.25 for 25%.</summary>
    Percent = 5,
    /// <summary>A date without a time.</summary>
    Date = 6,
    /// <summary>A date with a time.</summary>
    DateTime = 7
}
