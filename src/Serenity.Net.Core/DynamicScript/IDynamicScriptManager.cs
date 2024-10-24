namespace Serenity.Web;

/// <summary>
/// Dynamic script manager abstraction
/// </summary>
public interface IDynamicScriptManager
{
    /// <summary>
    /// Event that is raised when a script changed
    /// </summary>
    event Action<string> ScriptChanged;

    /// <summary>
    /// Raises script changed event for the script
    /// </summary>
    /// <param name="name">Registration name</param>
    void Changed(string name);

    /// <summary>
    /// Checks access rights for the dynamic script
    /// </summary>
    /// <param name="name">Registration name</param>
    void CheckScriptRights(string name);

    /// <summary>
    /// Gets a dictionary of registered script names and 
    /// their cached hashes
    /// </summary>
    /// <returns></returns>
    Dictionary<string, string> GetRegisteredScripts();

    /// <summary>
    /// Gets a list of registered script names
    /// </summary>
    IEnumerable<string> GetRegisteredScriptNames();

    /// <summary>
    /// Gets a script include element HTML markup
    /// </summary>
    /// <param name="name">Registered name</param>
    /// <param name="extension">Expected extension, default is ".js"</param>
    string GetScriptInclude(string name, string extension = ".js");

    /// <summary>
    /// Gets the dynamic script
    /// </summary>
    /// <param name="name">Registered name</param>
    /// <param name="json">True to get JSON data</param>
    string GetScriptText(string name, bool json = false);

    /// <summary>
    /// Reads content of a dynamic script
    /// </summary>
    /// <param name="name">Registered name</param>
    /// <param name="json">True to return JSON</param>
    IScriptContent ReadScriptContent(string name, bool json = false);

    /// <summary>
    /// Executes callback if a script with the name is not already registered
    /// </summary>
    /// <param name="name">Registered name</param>
    /// <param name="callback">Callback</param>
    void IfNotRegistered(string name, Func<IDynamicScript> callback);

    /// <summary>
    /// Returns if a script with the name is registered
    /// </summary>
    /// <param name="name">Registration name</param>
    bool IsRegistered(string name);

    /// <summary>
    /// Registers a dynamic script, potentially overriding
    /// a script with same registration name
    /// </summary>
    /// <param name="script">Dynamic script</param>
    void Register(INamedDynamicScript script);

    /// <summary>
    /// Registers a dynamic script, potentially overriding
    /// a script with same registration name
    /// </summary>
    /// <param name="name">Name for the script</param>
    /// <param name="script">Dynamic script</param>
    void Register(string name, IDynamicScript script);

    /// <summary>
    /// Resets the dynamic script cache
    /// </summary>
    void Reset();
}