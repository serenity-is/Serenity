namespace Serenity.Web;

/// <summary>
/// Dynamic script abstraction
/// </summary>
public interface IDynamicScript
{
    /// <summary>
    /// Gets the script content
    /// </summary>
    string GetScript();

    /// <summary>
    /// Returns true if the current user has the permissions required to access this script.
    /// </summary>
    /// <param name="permissions">Permission service used to check the required permissions.</param>
    bool HasPermission(IPermissionService permissions);

    /// <summary>
    /// Group key for cached items
    /// </summary>
    string? GroupKey { get; }

    /// <summary>
    /// Cache expiration timespan
    /// </summary>
    TimeSpan Expiration { get; }
}