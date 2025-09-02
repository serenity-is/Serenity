namespace Serenity.Web;

/// <summary>
/// Abstraction for script bundling manager
/// </summary>
public interface IScriptBundleManager
{
    /// <summary>
    /// Returns true if bundling is enabled
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    /// Gets the list of bundle includes
    /// </summary>
    /// <param name="bundleKey">Bundle key</param>

    IEnumerable<string> GetBundleIncludes(string bundleKey);

    /// <summary>
    /// Gets the script bundle containing a script url
    /// </summary>
    /// <param name="scriptUrl">Script url</param>
    string GetScriptBundle(string scriptUrl);


    /// <summary>
    /// Resets the css bundle manager
    /// </summary>
    void Reset();
    
    /// <summary>
    /// A method that can be called to clear script file cache
    /// </summary>
    void ScriptsChanged();
}