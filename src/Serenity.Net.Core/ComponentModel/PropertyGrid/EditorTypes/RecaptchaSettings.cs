namespace Serenity.Web;

/// <summary>
/// Settings for Recaptcha integration
/// Get your keys from https://www.google.com/recaptcha/admin/ 
/// Generate keys for the V2, not V3
/// </summary>
[DefaultSectionKey(SectionKey)]
public class RecaptchaSettings
{
    /// <summary>Default section key for Recaptcha settings</summary>
    public const string SectionKey = "Recaptcha";

    /// <summary>
    /// Gets or sets the site key.
    /// </summary>
    /// <value>
    /// The site key.
    /// </value>
    public string? SiteKey { get; set; }

    /// <summary>
    /// Gets or sets the secret key.
    /// </summary>
    /// <value>
    /// The secret key.
    /// </value>
    public string? SecretKey { get; set; }
}