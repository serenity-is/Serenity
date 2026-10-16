namespace Serenity.Plugins;

/// <summary>
/// A CSS file dependency for plugins.
/// </summary>
/// <remarks>
/// This type is part of a legacy plugin system design and is no longer used by
/// the framework. Use the <c>LayoutCssFile</c> assembly attribute instead to
/// declare CSS files that should be included in the layout.
/// </remarks>
public class CssFile
{
    /// <summary>
    /// Gets or sets the path. Should start with "./" corresponding to the plugin folder.
    /// </summary>
    /// <value>
    /// The path.
    /// </value>
    public string? Path { get; set; }
}