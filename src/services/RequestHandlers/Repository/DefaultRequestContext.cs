using Microsoft.Extensions.Options;

namespace Serenity.Services;

/// <summary>
/// Default implementation for a <see cref="IRequestContext"/>.
/// </summary>
/// <remarks>
/// <para>This implementation is registered as a singleton by default, so it assumes its
/// dependencies (e.g. <see cref="IUserAccessor"/>, <see cref="IPermissionService"/>,
/// <see cref="ITextLocalizer"/>) are also registered as singletons. The default
/// <see cref="IUserAccessor"/> implementation in web applications reads the current user from
/// the ambient HTTP context, so a singleton is safe in that case.</para>
/// <para>If you need scoped implementations of any of these dependencies, register your own
/// <see cref="IRequestContext"/> implementation and resolve them on demand from
/// <see cref="IServiceProviderAccessor.ServiceProvider"/> (which returns the current request's
/// scoped provider in web applications, and the root provider otherwise) instead of capturing
/// them in the constructor.</para>
/// </remarks>
/// <param name="behaviors">Behavior provider</param>
/// <param name="cache">Two level cache</param>
/// <param name="localizer">Text localizer</param>
/// <param name="permissions">Permissions</param>
/// <param name="userAccessor">User access</param>
/// <param name="handlerSettings">Request handler settings</param>
/// <exception cref="ArgumentNullException">Any of the arguments is <c>null</c>.</exception>
public class DefaultRequestContext(IBehaviorProvider behaviors, ITwoLevelCache cache, ITextLocalizer localizer,
    IPermissionService permissions, IUserAccessor userAccessor,
    IOptions<RequestHandlerSettings>? handlerSettings = null) : IRequestContext, IHasHandlerSettings
{
    private readonly IUserAccessor userAccessor = userAccessor ?? throw new ArgumentNullException(nameof(userAccessor));

    /// <inheritdoc/>
    public IBehaviorProvider Behaviors { get; private set; } = behaviors ?? throw new ArgumentNullException(nameof(behaviors));
    /// <inheritdoc/>
    public ITwoLevelCache Cache { get; private set; } = cache ?? throw new ArgumentNullException(nameof(cache));
    /// <inheritdoc/>
    public RequestHandlerSettings HandlerSettings => handlerSettings?.Value ?? RequestHandlerSettings.Default;
    /// <inheritdoc/>
    public ITextLocalizer Localizer { get; private set; } = localizer ?? throw new ArgumentNullException(nameof(localizer));
    /// <inheritdoc/>
    public IPermissionService Permissions { get; private set; } = permissions ?? throw new ArgumentNullException(nameof(permissions));
    /// <inheritdoc/>
    public ClaimsPrincipal? User => userAccessor.User;
}

