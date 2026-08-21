namespace Serenity.Reporting;

/// <summary>
/// Marks a class as a report.
/// </summary>
/// <remarks>
/// Initializes a new instance of the attribute.
/// </remarks>
/// <param name="reportKey">The report key. If not passed
/// it is calculated from the class name.</param>
public class ReportAttribute(string reportKey = null) : Attribute
{

    /// <summary>
    /// Gets the report key.
    /// </summary>
    public string ReportKey { get; private set; } = reportKey;
}