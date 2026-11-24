namespace Serenity.Reporting;

/// <summary>
/// Marks a class as a report.
/// </summary>
/// <remarks>
/// Initializes a new instance of the attribute.
/// </remarks>
/// <param name="reportKey">The report key. If not passed
/// it is calculated from the class name.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ReportAttribute(string? reportKey = null) : Attribute
{
    /// <summary>
    /// Gets the report key.
    /// </summary>
    public string? ReportKey { get; private set; } = reportKey;

    /// <summary>
    /// Gets or sets the order of the report in its category, for a page that lists reports in groups.
    /// </summary>
    public int Order { get; set; }
}