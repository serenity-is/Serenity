namespace Serenity.Web;

/// <summary>
/// Abstraction for css bundling manager
/// </summary>
public interface ICssBundleManager
{
    /// <summary>
    /// Returns true if bundling is enabled
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    /// A method that can be called to clear css file cache
    /// </summary>
    void CssChanged();

    /// <summary>
    /// Gets the list of bundle includes
    /// </summary>
    /// <param name="bundleKey">Bundle key</param>
    IEnumerable<string> GetBundleIncludes(string bundleKey);

    /// <summary>
    /// Gets the CSS bundle containing a css url
    /// </summary>
    /// <param name="cssUrl">CSS url</param>
    string GetCssBundle(string cssUrl);

    /// <summary>
    /// Resets the css bundle manager
    /// </summary>
    void Reset();
}