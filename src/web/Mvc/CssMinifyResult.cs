namespace Serenity.Web;

/// <summary>
/// Result returned from a CSS minifier
/// </summary>
public class CssMinifyResult
{
    /// <summary>
    /// Gets the minified code
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Gets if minifying had errors
    /// </summary>
    public bool HasErrors { get; set; }
}