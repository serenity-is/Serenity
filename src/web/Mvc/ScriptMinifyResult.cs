namespace Serenity.Web;

/// <summary>
/// Result returned from a script minifier.
/// </summary>
public class ScriptMinifyResult
{
    /// <summary>
    /// Gets or sets the minified code.
    /// </summary>
    public required string Code { get; set; }

    /// <summary>
    /// Gets or sets whether minifying had errors.
    /// </summary>
    public bool HasErrors { get; set; }
}