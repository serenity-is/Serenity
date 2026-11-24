namespace Serenity.Reporting;

/// <summary>
/// How the total of a report column is computed.
/// </summary>
public enum ReportTotalKind
{
    /// <summary>The column has no total.</summary>
    None = 0,
    /// <summary>The sum of the values.</summary>
    Sum = 1,
    /// <summary>The average of the values that are not null.</summary>
    Average = 2
}
