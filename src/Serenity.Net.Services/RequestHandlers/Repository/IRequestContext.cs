namespace Serenity.Services;

/// <summary>
/// A service request context abstraction containing 
/// services that are mostly used by all service
/// handler types. This is a type similar to the HttpContext
/// but is specialized for service handlers.
/// </summary>
public interface IRequestContext
{
    /// <summary>
    /// Gets the behavior provider
    /// </summary>
    IBehaviorProvider Behaviors { get; }

    /// <summary>
    /// Get the two level cache
    /// </summary>
    ITwoLevelCache Cache { get; }

    /// <summary>
    /// Gets the text localizer
    /// </summary>
    ITextLocalizer Localizer { get; }

    /// <summary>
    /// Gets the permission service
    /// </summary>
    IPermissionService Permissions { get; }

    /// <summary>
    /// Gets current user
    /// </summary>
    ClaimsPrincipal User { get; }
}