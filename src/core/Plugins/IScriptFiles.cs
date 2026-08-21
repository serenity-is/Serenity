namespace Serenity.Plugins;

/// <summary>
/// Abstraction for plugins with script file dependencies.
/// </summary>
public interface IScriptFiles
{
    /// <summary>
    /// Gets the script files.
    /// </summary>
    /// <returns>The list of script files.</returns>
    IEnumerable<ScriptFile> GetScriptFiles();
}