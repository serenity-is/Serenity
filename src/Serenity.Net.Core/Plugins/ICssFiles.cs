namespace Serenity.Plugins;

/// <summary>
/// CSS file dependencies for plugins.
/// </summary>
public interface ICssFiles
{
    /// <summary>
    /// Gets the list of CSS files.
    /// </summary>
    /// <returns>List of CSS files.</returns>
    IEnumerable<CssFile> GetCssFiles();
}