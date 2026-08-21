namespace Serenity.Web;

/// <summary>
/// Abstraction for a script bundling manager.
/// </summary>
public interface IScriptBundleManager
{
    /// <summary>
    /// Returns <c>true</c> if bundling is enabled.
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    /// Gets the list of bundle includes.
    /// </summary>
    /// <param name="bundleKey">The bundle key.</param>
    /// <returns>The list of bundle includes.</returns>
    IEnumerable<string> GetBundleIncludes(string bundleKey);

    /// <summary>
    /// Gets the script bundle containing a script URL.
    /// </summary>
    /// <param name="scriptUrl">The script URL.</param>
    /// <returns>The script bundle URL.</returns>
    string GetScriptBundle(string scriptUrl);

    /// <summary>
    /// Resets the script bundle manager.
    /// </summary>
    void Reset();
    
    /// <summary>
    /// A method that can be called to clear the script file cache.
    /// </summary>
    void ScriptsChanged();
}