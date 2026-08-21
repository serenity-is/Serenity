namespace Serenity.Reporting;

/// <summary>
/// Marks a report to determine if it should use WKHTML instead
/// of another converter, e.g. Puppeteer etc. (only if configured
/// in service provider).
/// </summary>
/// <remarks>
/// Initializes a new instance of the attribute.
/// </remarks>
/// <param name="value">Whether the report should use WKHTML to PDF.</param>
public class UseWKHtmlToPdfAttribute(bool value = true) : Attribute
{

    /// <summary>
    /// Gets a value indicating whether WKHTML to PDF should be used.
    /// </summary>
    public bool Value { get; private set; } = value;
}