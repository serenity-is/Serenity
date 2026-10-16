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
}
