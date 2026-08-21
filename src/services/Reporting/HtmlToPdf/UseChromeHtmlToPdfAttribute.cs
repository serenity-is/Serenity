namespace Serenity.Reporting;

/// <summary>
/// Marks a report so that it should use Chrome instead of WKHTML
/// </summary>
[Obsolete("Prefer UseWKHtmlToPdfAttribute(false)")]
public class UseChromeHtmlToPdfAttribute : UseWKHtmlToPdfAttribute
{
    /// <summary>
    /// Initializes a new instance of the attribute with <c>false</c> as the value.
    /// </summary>
    public UseChromeHtmlToPdfAttribute()
        : base(false)
    {
    }
}