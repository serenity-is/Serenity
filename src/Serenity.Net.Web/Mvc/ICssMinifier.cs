namespace Serenity.Web;

/// <summary>
/// Abstraction for Css minifier
/// </summary>
public interface ICssMinifier
{
    /// <summary>
    /// Minifies the given Css
    /// </summary>
    /// <param name="source">JavaCss content</param>
    /// <param name="options">Minify options</param>
    /// <returns>Minify result</returns>
    CssMinifyResult MinifyCss(string source, CssMinifyOptions options);
}