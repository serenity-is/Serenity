namespace Serenity.Services;

/// <summary>
/// Default implementation for a <see cref="IRequestContext"/>
/// </summary>
/// <remarks>
/// Creates an instance of the class
/// </remarks>
/// <param name="behaviors">Behavior provider</param>
/// <param name="cache">Two level cache</param>
/// <param name="localizer">Text localizer</param>
/// <param name="permissions">Permissions</param>
/// <param name="userAccessor">User access</param>
/// <exception cref="ArgumentNullException">Any of the arguments is null</exception>
public class DefaultRequestContext(IBehaviorProvider behaviors, ITwoLevelCache cache, ITextLocalizer localizer,
    IPermissionService permissions, IUserAccessor userAccessor) : IRequestContext
{
    private readonly IUserAccessor userAccessor = userAccessor ?? throw new ArgumentNullException(nameof(userAccessor));

    /// <inheritdoc/>
    public IBehaviorProvider Behaviors { get; private set; } = behaviors ?? throw new ArgumentNullException(nameof(behaviors));
    /// <inheritdoc/>
    public ITwoLevelCache Cache { get; private set; } = cache ?? throw new ArgumentNullException(nameof(cache));
    /// <inheritdoc/>
    public ITextLocalizer Localizer { get; private set; } = localizer ?? throw new ArgumentNullException(nameof(localizer));
    /// <inheritdoc/>
    public IPermissionService Permissions { get; private set; } = permissions ?? throw new ArgumentNullException(nameof(permissions));
    /// <inheritdoc/>
    public ClaimsPrincipal User => userAccessor.User;
}

