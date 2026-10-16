namespace Serenity.Plugins;

/// <summary>
/// Abstraction for plugins with script file dependencies.
/// </summary>
/// <remarks>
/// This type is part of a legacy plugin system design and is no longer used by
/// the framework. Use the <c>LayoutScriptFile</c> assembly attribute instead to
/// declare scripts that should be included in the layout.
/// </remarks>
public interface IScriptFiles
{
    /// <summary>
    /// Gets the script files.
    /// </summary>
    /// <returns>The list of script files.</returns>
    IEnumerable<ScriptFile> GetScriptFiles();
}