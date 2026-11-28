using System.Security.Claims;

namespace Serenity.Extensions;

/// <summary>
/// Sends the invitations to sign up, and accepts them, see <see cref="Invitation"/>. Who can invite whom, with which
/// roles, is decided by the <see cref="IInvitationPolicy"/>s. The application creates the user when an invitation is
/// accepted, on its sign-up page, and gives the user the roles of <see cref="Accept"/>. Pro registers one with
/// <c>AddInvitations</c>.
/// </summary>
public interface IInvitationManager
{
    /// <summary>Whether the invitations are turned on, e.g. by the "Invitations:Enabled" setting.</summary>
    bool IsEnabled { get; }

    /// <summary>
    /// Gets why the inviter can't send an invitation, localized, or null when the inviter can.
    /// </summary>
    /// <param name="inviter">The user who would invite, who is the current user.</param>
    /// <param name="invitation">The invitation, with the roles.</param>
    string? GetDenial(ClaimsPrincipal inviter, InvitationDraft invitation);

    /// <summary>
    /// Invites someone, as the current user, and emails the link. It throws a validation error when the current user
    /// can't, the email address belongs to a user, or has a pending invitation, which can be sent again instead.
    /// </summary>
    /// <param name="invitation">Who is invited, with the roles.</param>
    /// <returns>The invitation.</returns>
    Invitation Invite(InvitationDraft invitation);

    /// <summary>
    /// Sends a pending, or an expired, invitation again, with a new link, so the old one no longer works.
    /// </summary>
    /// <param name="invitationId">The ID of the invitation.</param>
    void Resend(long invitationId);

    /// <summary>
    /// Revokes a pending invitation, so its link no longer works.
    /// </summary>
    /// <param name="invitationId">The ID of the invitation.</param>
    void Revoke(long invitationId);

    /// <summary>
    /// Deletes invitations that are no longer pending: accepted, revoked or expired. A pending one is revoked first.
    /// </summary>
    /// <param name="invitationIds">The IDs of the invitations.</param>
    /// <returns>The number of the invitations that were deleted.</returns>
    int Delete(IEnumerable<long> invitationIds);

    /// <summary>
    /// Gets the invitation of a link, or null when it is not one that can be accepted, e.g. expired or used.
    /// </summary>
    /// <param name="token">The token of the link.</param>
    Invitation? FindPending(string token);

    /// <summary>
    /// Accepts the invitation of a link for a user that the application creates with its email address, and gives the
    /// roles that the user gets: the policies are asked as the inviter for each role, and the ones the inviter can no
    /// longer give are left out. The invitation can be accepted once; it can again if the unit of work rolls back.
    /// </summary>
    /// <param name="token">The token of the link.</param>
    /// <param name="userId">The ID of the new user.</param>
    /// <param name="unitOfWork">The unit of work that creates the user, if any, after which the event is published.</param>
    /// <returns>The invitation, with the roles.</returns>
    InvitationAcceptance Accept(string token, string userId, IUnitOfWork? unitOfWork = null);
}
