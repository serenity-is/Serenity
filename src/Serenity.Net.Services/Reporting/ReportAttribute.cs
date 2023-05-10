namespace Serenity.Reporting;

/// <summary>
/// Marks a class as Report
/// </summary>
public class ReportAttribute : Attribute
{
    /// <summary>
    /// Creates an instance of the attribute
    /// </summary>
    /// <param name="reportKey">The report key. If not passed
    /// it is calculated from the class name.</param>
    public ReportAttribute(string reportKey = null)
    {
        ReportKey = reportKey;
    }

    /// <summary>
    /// The report key.
    /// </summary>
    public string ReportKey { get; private set; }
}