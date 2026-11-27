using System.Security.Claims;

namespace Serenity.Extensions;

/// <summary>
/// Tells whether a user is impersonated, and by whom.
/// </summary>
public static class ImpersonationExtensions
{
    /// <summary>The error code of an action that is not available while impersonating.</summary>
    public const string RestrictedCode = "ImpersonationRestricted";

    /// <summary>
    /// Gets the identity of the user who impersonates the user of the principal, or null.
    /// </summary>
    /// <param name="user">The principal.</param>
    public static ClaimsIdentity? GetImpersonator(this ClaimsPrincipal? user) =>
        user?.Identity is ClaimsIdentity { IsAuthenticated: true, Actor: ClaimsIdentity actor } ? actor : null;

    /// <summary>
    /// Whether the user of the principal is impersonated by another user.
    /// </summary>
    /// <param name="user">The principal.</param>
    public static bool IsImpersonating(this ClaimsPrincipal? user) => user.GetImpersonator() != null;

    /// <summary>
    /// Throws a validation error with the <see cref="RestrictedCode"/> code while the user is impersonated, e.g. for an
    /// action on the security of the account, which the impersonator should not do as the user.
    /// </summary>
    /// <param name="user">The principal.</param>
    /// <param name="localizer">The localizer of the message.</param>
    public static void ValidateNotImpersonating(this ClaimsPrincipal? user, ITextLocalizer? localizer)
    {
        if (user.IsImpersonating())
            throw new ValidationError(RestrictedCode, ImpersonationTexts.Restricted.ToString(localizer));
    }
}
