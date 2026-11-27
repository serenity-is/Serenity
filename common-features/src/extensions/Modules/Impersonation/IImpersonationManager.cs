using System.Security.Claims;

namespace Serenity.Extensions;

/// <summary>
/// Decides whether a user can impersonate another one, from its options, the checks that always apply, and the
/// <see cref="IImpersonationPolicy"/>s. Pro registers one with <c>AddImpersonation</c>.
/// </summary>
public interface IImpersonationManager
{
    /// <summary>Whether impersonation is turned on, e.g. by the "Impersonation:Enabled" setting.</summary>
    bool IsEnabled { get; }

    /// <summary>
    /// Gets why the impersonator can't impersonate the target, localized, or null when the impersonator can.
    /// </summary>
    /// <param name="impersonator">The user who would impersonate, who is the current user.</param>
    /// <param name="target">The user who would be impersonated.</param>
    string? GetDenial(ClaimsPrincipal impersonator, IUserDefinition target);
}
