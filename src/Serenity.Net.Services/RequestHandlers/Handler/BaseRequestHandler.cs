namespace Serenity.Services;

/// <summary>
/// An abstract class that can be used as base for request handlers 
/// that accept a <see cref="IRequestContext"/> instance.
/// </summary>
public abstract class BaseRequestHandler : IRequestHandler
{
    /// <summary>
    /// Creates an instance of the class.
    /// </summary>
    /// <param name="context">Request context</param>
    /// <exception cref="ArgumentNullException">Context is null</exception>
    public BaseRequestHandler(IRequestContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Gets cache from the request context
    /// </summary>
    protected ITwoLevelCache Cache => Context.Cache;

    /// <summary>
    /// Gets the request context
    /// </summary>
    protected IRequestContext Context { get; }

    /// <summary>
    /// Gets text localizer from the request context
    /// </summary>
    protected ITextLocalizer Localizer => Context.Localizer;
    
    /// <summary>
    /// Gets permission service from the request context
    /// </summary>
    protected IPermissionService Permissions => Context.Permissions;

    /// <summary>
    /// Gets the user from the request context
    /// </summary>
    protected ClaimsPrincipal User => Context.User;
}