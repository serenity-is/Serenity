namespace Serenity.Web;

/// <summary>
/// Result returned from a CSS minifier.
/// </summary>
public class CssMinifyResult
{
    /// <summary>
    /// Gets or sets the minified code.
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Gets or sets whether minifying had errors.
    /// </summary>
    public bool HasErrors { get; set; }
}