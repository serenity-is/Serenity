namespace Serenity.Plugins;

/// <summary>
/// CSS file dependencies for plugins.
/// </summary>
/// <remarks>
/// This type is part of a legacy plugin system design and is no longer used by
/// the framework. Use the <c>LayoutCssFile</c> assembly attribute instead to
/// declare CSS files that should be included in the layout.
/// </remarks>
public interface ICssFiles
{
    /// <summary>
    /// Gets the list of CSS files.
    /// </summary>
    /// <returns>The list of CSS files.</returns>
    IEnumerable<CssFile> GetCssFiles();
}