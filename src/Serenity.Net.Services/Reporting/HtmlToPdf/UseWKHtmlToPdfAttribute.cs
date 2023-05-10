namespace Serenity.Reporting;

/// <summary>
/// Marks a report to determine if it should use WKHTML instead
/// of another converter, e.g. Puppeeteer etc. (only if configured
/// in service provider).
/// </summary>
public class UseWKHtmlToPdfAttribute : Attribute
{
    /// <summary>
    /// Creates a new instance of the attribute
    /// </summary>
    /// <param name="value"></param>
    public UseWKHtmlToPdfAttribute(bool value = true)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the value
    /// </summary>
    public bool Value { get; private set; }
}