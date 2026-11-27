using System.Security.Claims;

namespace Serenity.Extensions;

/// <summary>
/// Decides whether a user can impersonate another one, see <see cref="IImpersonationManager"/>: a denial of any policy
/// wins, else at least one must allow it, so nobody can impersonate unless a policy says so. The default one of Pro
/// (<c>DefaultImpersonationPolicy</c>) allows the holders of a permission, and denies the super admins as targets;
/// an application adds its own, e.g. one that lets a teacher impersonate the own students, with
/// <c>services.TryAddEnumerable(ServiceDescriptor.Singleton&lt;IImpersonationPolicy, MyPolicy&gt;())</c>.
/// </summary>
public interface IImpersonationPolicy
{
    /// <summary>
    /// Decides whether the impersonator, the current user, can impersonate the target.
    /// </summary>
    /// <param name="impersonator">The user who would impersonate, who is the current user.</param>
    /// <param name="target">The user who would be impersonated.</param>
    /// <param name="reason">Why, for a denial, localized.</param>
    ImpersonationDecision Check(ClaimsPrincipal impersonator, IUserDefinition target, out string? reason);
}
