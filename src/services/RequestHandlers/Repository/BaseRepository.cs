namespace Serenity.Services;

/// <summary>
/// A base class that can be used for repositories (obsolete, <see cref="BaseRequestHandler"/>)
/// that accept a <see cref="IRequestContext"/> instance.
/// </summary>
/// <remarks>
/// Initializes a new instance of the class.
/// </remarks>
/// <param name="context">Request context</param>
/// <exception cref="ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
public class BaseRepository(IRequestContext context)
{

    /// <summary>
    /// Gets the cache from the request context.
    /// </summary>
    protected ITwoLevelCache Cache => Context.Cache;

    /// <summary>
    /// Gets the request context.
    /// </summary>
    protected IRequestContext Context { get; } = context ?? throw new ArgumentNullException(nameof(context));

    /// <summary>
    /// Gets the text localizer from the request context.
    /// </summary>
    protected ITextLocalizer Localizer => Context.Localizer;

    /// <summary>
    /// Gets the permission service from the request context.
    /// </summary>
    protected IPermissionService Permissions => Context.Permissions;

    /// <summary>
    /// Gets the user from the request context.
    /// </summary>
    protected ClaimsPrincipal User => Context.User;
}