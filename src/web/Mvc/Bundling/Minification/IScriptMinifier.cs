namespace Serenity.Web;

/// <summary>
/// Abstraction for a script minifier.
/// </summary>
public interface IScriptMinifier
{
    /// <summary>
    /// Minifies the given script.
    /// </summary>
    /// <param name="source">The JavaScript content.</param>
    /// <param name="options">The minify options.</param>
    /// <returns>The minify result.</returns>
    ScriptMinifyResult MinifyScript(string source, ScriptMinifyOptions options);
}