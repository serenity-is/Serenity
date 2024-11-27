namespace Serenity.Web;

/// <summary>
/// Base abstract implementation for <see cref="IDynamicScript"/>
/// </summary>
public abstract class DynamicScript : IDynamicScript
{
    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    protected DynamicScript()
    {
    }

    /// <inheritdoc/>
    public abstract string GetScript();

    /// <inheritdoc/>
    public virtual void CheckRights(IPermissionService permissions, ITextLocalizer localizer)
    {
        if (Permission != null)
            permissions.ValidatePermission(Permission, localizer);
    }

    /// <inheritdoc/>
    public string GroupKey { get; set; }
    
    /// <inheritdoc/>
    public TimeSpan Expiration { get; set; }
    
    /// <inheritdoc/>
    public string Permission { get; set; }
}