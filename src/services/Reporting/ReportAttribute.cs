namespace Serenity.Reporting;

/// <summary>
/// Marks a class as Report
/// </summary>
/// <remarks>
/// Creates an instance of the attribute
/// </remarks>
/// <param name="reportKey">The report key. If not passed
/// it is calculated from the class name.</param>
public class ReportAttribute(string reportKey = null) : Attribute
{

    /// <summary>
    /// The report key.
    /// </summary>
    public string ReportKey { get; private set; } = reportKey;
}