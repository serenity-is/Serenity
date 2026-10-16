namespace Serenity.Web;

/// <summary>
/// Abstraction for a CSS minifier.
/// </summary>
public interface ICssMinifier
{
    /// <summary>
    /// Minifies the given CSS.
    /// </summary>
    /// <param name="source">The CSS content.</param>
    /// <param name="options">The minify options.</param>
    /// <returns>The minify result.</returns>
    CssMinifyResult MinifyCss(string source, CssMinifyOptions options);
}