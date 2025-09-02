namespace Serenity.Reporting;

/// <summary>
/// Marks a report to determine if it should use WKHTML instead
/// of another converter, e.g. Puppeteer etc. (only if configured
/// in service provider).
/// </summary>
/// <remarks>
/// Creates a new instance of the attribute
/// </remarks>
/// <param name="value"></param>
public class UseWKHtmlToPdfAttribute(bool value = true) : Attribute
{

    /// <summary>
    /// Gets the value
    /// </summary>
    public bool Value { get; private set; } = value;
}