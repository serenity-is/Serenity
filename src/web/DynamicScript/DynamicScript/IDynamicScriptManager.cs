namespace Serenity.Web;

/// <summary>
/// Dynamic script manager abstraction.
/// </summary>
public interface IDynamicScriptManager
{
    /// <summary>
    /// Raised when a script changes.
    /// </summary>
    event Action<string> ScriptChanged;

    /// <summary>
    /// Raises the script changed event for the script.
    /// </summary>
    /// <param name="name">The registration name.</param>
    void Changed(string name);

    /// <summary>
    /// Checks access rights for the dynamic script.
    /// </summary>
    /// <param name="name">The registration name.</param>
    void CheckScriptRights(string name);

    /// <summary>
    /// Gets a dictionary of registered script names and their cached hashes.
    /// </summary>
    /// <returns>A dictionary of script names and hashes.</returns>
    Dictionary<string, string> GetRegisteredScripts();

    /// <summary>
    /// Gets a list of registered script names.
    /// </summary>
    /// <returns>The list of registered script names.</returns>
    IEnumerable<string> GetRegisteredScriptNames();

    /// <summary>
    /// Gets a script include element HTML markup.
    /// </summary>
    /// <param name="name">The registered name.</param>
    /// <param name="extension">The expected extension; defaults to <c>.js</c>.</param>
    /// <returns>The script include markup.</returns>
    string GetScriptInclude(string name, string extension = ".js");

    /// <summary>
    /// Gets the dynamic script text.
    /// </summary>
    /// <param name="name">The registered name.</param>
    /// <param name="json"><c>true</c> to get JSON data.</param>
    /// <returns>The script text.</returns>
    string GetScriptText(string name, bool json = false);

    /// <summary>
    /// Reads the content of a dynamic script.
    /// </summary>
    /// <param name="name">The registered name.</param>
    /// <param name="json"><c>true</c> to return JSON.</param>
    /// <returns>The script content.</returns>
    IScriptContent ReadScriptContent(string name, bool json = false);

    /// <summary>
    /// Executes the callback if a script with the name is not already registered.
    /// </summary>
    /// <param name="name">The registered name.</param>
    /// <param name="callback">The callback to execute.</param>
    void IfNotRegistered(string name, Func<IDynamicScript> callback);

    /// <summary>
    /// Returns whether a script with the name is registered.
    /// </summary>
    /// <param name="name">The registration name.</param>
    /// <returns><c>true</c> if the script is registered; otherwise, <c>false</c>.</returns>
    bool IsRegistered(string name);

    /// <summary>
    /// Registers a dynamic script, potentially overriding a script with the
    /// same registration name.
    /// </summary>
    /// <param name="script">The dynamic script.</param>
    void Register(INamedDynamicScript script);

    /// <summary>
    /// Registers a dynamic script, potentially overriding a script with the
    /// same registration name.
    /// </summary>
    /// <param name="name">The name for the script.</param>
    /// <param name="script">The dynamic script.</param>
    void Register(string name, IDynamicScript script);

    /// <summary>
    /// Resets the dynamic script cache.
    /// </summary>
    void Reset();
}