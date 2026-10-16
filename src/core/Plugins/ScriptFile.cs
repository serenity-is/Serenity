namespace Serenity.Plugins;

/// <summary>
/// A plugin script file registration.
/// </summary>
/// <remarks>
/// This type is part of a legacy plugin system design and is no longer used by
/// the framework. Use the <c>LayoutScriptFile</c> assembly attribute instead to
/// declare scripts that should be included in the layout.
/// </remarks>
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