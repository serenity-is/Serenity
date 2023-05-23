namespace Serenity.Web;

/// <summary>
/// Abstraction for script minifier
/// </summary>
public interface IScriptMinifier
{
    /// <summary>
    /// Minifies the given script
    /// </summary>
    /// <param name="source">Javascript content</param>
    /// <param name="options">Minify options</param>
    /// <returns>Minify result</returns>
    ScriptMinifyResult MinifyScript(string source, ScriptMinifyOptions options);
}