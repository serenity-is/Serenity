using Microsoft.Extensions.Primitives;

namespace Serenity.Web;

/// <summary>
/// Provides change tokens for web files (under <c>wwwroot</c> or static web
/// assets), usually only in development, by reusing physical file providers.
/// </summary>
public interface IWebRootFileWatcher
{
    /// <summary>
    /// Gets a change token that fires when the given web relative file changes,
    /// or <c>null</c> if watching is not available (e.g. not in development, or
    /// the file has no physical path).
    /// </summary>
    /// <param name="webRelativePath">The web relative path, e.g. <c>Scripts/site.js</c>.</param>
    /// <returns>A change token, or <c>null</c>.</returns>
    IChangeToken? GetChangeToken(string webRelativePath);

    /// <summary>
    /// Registers a callback that is invoked when any of the given app relative
    /// files changes. Only the first registration for a given key is honored, so
    /// callers may invoke this on every request without accumulating
    /// subscriptions. Only available in development.
    /// </summary>
    /// <param name="key">A key identifying the registration, to avoid duplicates.</param>
    /// <param name="appRelativePaths">The app relative (<c>~/</c>) file urls to watch.</param>
    /// <param name="callback">The callback to invoke on change.</param>
    void RegisterCallback(string key, IEnumerable<string> appRelativePaths, Action callback);
}
