namespace Serenity.Services;

/// <summary>
/// Default implementation for a <see cref="IRequestContext"/>
/// </summary>
public class DefaultRequestContext : IRequestContext
{
    private readonly IUserAccessor userAccessor;

    /// <summary>
    /// Creates an instance of the class
    /// </summary>
    /// <param name="behaviors">Behavior provider</param>
    /// <param name="cache">Two level cache</param>
    /// <param name="localizer">Text localizer</param>
    /// <param name="permissions">Permissions</param>
    /// <param name="userAccessor">User access</param>
    /// <exception cref="ArgumentNullException">Any of the arguments is null</exception>
    public DefaultRequestContext(IBehaviorProvider behaviors, ITwoLevelCache cache, ITextLocalizer localizer, 
        IPermissionService permissions, IUserAccessor userAccessor)
    {
        Behaviors = behaviors ?? throw new ArgumentNullException(nameof(behaviors));
        Cache = cache ?? throw new ArgumentNullException(nameof(cache));
        Localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        Permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
        this.userAccessor = userAccessor ?? throw new ArgumentNullException(nameof(userAccessor));
    }

    /// <inheritdoc/>
    public IBehaviorProvider Behaviors { get; private set; }
    /// <inheritdoc/>
    public ITwoLevelCache Cache { get; private set; }
    /// <inheritdoc/>
    public ITextLocalizer Localizer { get; private set; }
    /// <inheritdoc/>
    public IPermissionService Permissions { get; private set; }
    /// <inheritdoc/>
    public ClaimsPrincipal User => userAccessor.User;
}

