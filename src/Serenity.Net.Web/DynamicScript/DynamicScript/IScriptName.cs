
namespace Serenity.Web;

/// <summary>
/// Provides access to the registration name for a dynamic script
/// </summary>
public interface IScriptName
{
    /// <summary>
    /// Gets the script name
    /// </summary>
    string ScriptName { get; }
}