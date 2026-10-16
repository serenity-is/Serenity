namespace Serenity.Web;

/// <summary>
/// Watches the CSS files that make up a layout CSS bundle and invalidates the
/// bundle when one of them changes in development, so that the cached bundle
/// content is refreshed without requiring a restart.
/// </summary>
public interface ILayoutCssFileWatcher
{
    /// <summary>
    /// Ensures the layout CSS bundle script with the given name is watched, so
    /// that it is invalidated when any of the given files changes.
    /// </summary>
    /// <param name="scriptName">The dynamic script name of the bundle.</param>
    /// <param name="appRelativeUrls">The app relative (<c>~/</c>) CSS file urls in the bundle.</param>
    void Watch(string scriptName, IReadOnlyList<string> appRelativeUrls);
}
