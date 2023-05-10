namespace Serenity.Plugins;

/// <summary>
/// CSS file dependency for plugins
/// </summary>
public class CssFile
{
    /// <summary>
    /// Gets or sets the path. Should start with "./" corresponding to plugin folder.
    /// </summary>
    /// <value>
    /// The path.
    /// </value>
    public string? Path { get; set; }
}