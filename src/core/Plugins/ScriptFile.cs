namespace Serenity.Plugins;

/// <summary>
/// A plugin script file registration.
/// </summary>
public class ScriptFile
{
    /// <summary>
    /// Gets or sets the path to the file. Should start with "./" corresponding to the plugin folder.
    /// </summary>
    /// <value>
    /// The path.
    /// </value>
    public string? Path { get; set; }
}