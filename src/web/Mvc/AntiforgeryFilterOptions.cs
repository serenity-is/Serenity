namespace Serenity.Web;

/// <summary>
/// Options for AutoValidateAntiforgeryIgnoreBearerFilter.
/// </summary>
[DefaultSectionKey("AntiforgeryFilter")]
public class AntiforgeryFilterOptions
{
    /// <summary>
    /// Gets or sets the name of the HTTP header that, if present in a request, causes the CSRF validation to be skipped.
    /// </summary>
    public string SkipValidationHeaderName { get; set; } = "X-CSRF-SKIP";

    /// <summary>
    /// Gets or sets the header value for <see cref="SkipValidationHeaderName"/> key that, if present in a request, causes the operation to be skipped.
    /// </summary>
    public string SkipValidationHeaderValue { get; set; } = "true";
}
