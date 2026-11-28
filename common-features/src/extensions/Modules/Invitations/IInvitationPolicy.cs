using System.Security.Claims;

namespace Serenity.Extensions;

/// <summary>
/// Decides whether a user can invite someone, with the roles of the invitation, see <see cref="IInvitationManager"/>: a
/// denial of any policy wins, else at least one must allow it, so nobody can invite unless a policy says so. The default
/// one of Pro (<c>DefaultInvitationPolicy</c>) allows the holders of a permission to invite with the roles whose
/// permissions they could grant; an application adds its own, e.g. one that lets a teacher invite students into the
/// students role, with <c>services.TryAddEnumerable(ServiceDescriptor.Singleton&lt;IInvitationPolicy, MyPolicy&gt;())</c>.
/// The policies are asked again for each role when the invitation is accepted, as the inviter, and the roles that the
/// inviter can no longer give are left out.
/// </summary>
public interface IInvitationPolicy
{
    /// <summary>
    /// Decides whether the inviter, the current user, can send the invitation.
    /// </summary>
    /// <param name="inviter">The user who invites, who is the current user.</param>
    /// <param name="invitation">The invitation, with the roles.</param>
    /// <param name="reason">Why, for a denial, localized.</param>
    InvitationDecision Check(ClaimsPrincipal inviter, InvitationDraft invitation, out string? reason);
}
