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
    /// Checks script permissions
    /// </summary>
    /// <param name="permissions"></param>
    /// <param name="localizer">Text localizer</param>
    void CheckRights(IPermissionService permissions, ITextLocalizer localizer);

    /// <summary>
    /// Group key for cached items
    /// </summary>
    string GroupKey { get; }

    /// <summary>
    /// Cache expiration timespan
    /// </summary>
    TimeSpan Expiration { get; }
}