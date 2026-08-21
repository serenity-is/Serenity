namespace Serenity.Web;

/// <summary>
/// Abstraction for a CSS bundling manager.
/// </summary>
public interface ICssBundleManager
{
    /// <summary>
    /// Returns <c>true</c> if bundling is enabled.
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    /// A method that can be called to clear the CSS file cache.
    /// </summary>
    void CssChanged();

    /// <summary>
    /// Gets the list of bundle includes.
    /// </summary>
    /// <param name="bundleKey">The bundle key.</param>
    /// <returns>The list of bundle includes.</returns>
    IEnumerable<string> GetBundleIncludes(string bundleKey);

    /// <summary>
    /// Gets the CSS bundle containing a CSS URL.
    /// </summary>
    /// <param name="cssUrl">The CSS URL.</param>
    /// <returns>The CSS bundle URL.</returns>
    string GetCssBundle(string cssUrl);

    /// <summary>
    /// Resets the CSS bundle manager.
    /// </summary>
    void Reset();
}