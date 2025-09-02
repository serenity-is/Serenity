namespace Serenity.Plugins;

/// <summary>
/// Abstraction for plugins with script file dependencies
/// </summary>
public interface IScriptFiles
{
    /// <summary>
    /// Gets the script files.
    /// </summary>
    /// <returns>List of script files.</returns>
    IEnumerable<ScriptFile> GetScriptFiles();
}