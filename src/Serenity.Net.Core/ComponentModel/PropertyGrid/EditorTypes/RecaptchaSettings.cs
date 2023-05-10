using Microsoft.Extensions.Options;

namespace Serenity.ComponentModel;

/// <summary>
/// Settings for Recaptcha integration
/// </summary>
public class RecaptchaSettings : IOptions<RecaptchaSettings>
{
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

    /// <summary>
    /// Returns this
    /// </summary>
    public RecaptchaSettings Value => this;
}