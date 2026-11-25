namespace Serenity.Web;

/// <summary>
/// Base abstract implementation for <see cref="IDynamicScript"/>.
/// </summary>
public abstract class DynamicScript : IDynamicScript
{
    /// <summary>
    /// Creates a new instance of the class.
    /// </summary>
    protected DynamicScript()
    {
    }

    /// <inheritdoc/>
    public abstract string GetScript();

    /// <summary>
    /// Returns true if the current user has the <see cref="Permission"/>.
    /// Access is denied if no <see cref="Permission"/> is set.
    /// </summary>
    /// <param name="permissions">Permission service</param>
    public virtual bool HasPermission(IPermissionService permissions)
    {
        ArgumentNullException.ThrowIfNull(permissions);
        return permissions.HasPermission(string.IsNullOrEmpty(Permission) ?
            SpecialPermissionKeys.Deny : Permission);
    }

    /// <inheritdoc/>
    public string? GroupKey { get; set; }
    
    /// <inheritdoc/>
    public TimeSpan Expiration { get; set; }
    
    /// <inheritdoc/>
    public string? Permission { get; set; }
}