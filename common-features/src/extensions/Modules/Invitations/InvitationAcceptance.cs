namespace Serenity.Extensions;

/// <summary>
/// The result of accepting an invitation, see <see cref="IInvitationManager.Accept"/>: the roles that the application gives
/// the new user, and the ones that were left out, as the inviter can no longer give them.
/// </summary>
public class InvitationAcceptance
{
    /// <summary>The invitation, which is accepted now.</summary>
    public Invitation Invitation { get; set; } = new();

    /// <summary>The IDs of the roles that the new user gets.</summary>
    public List<string> RoleIds { get; set; } = [];

    /// <summary>The IDs of the roles of the invitation that the inviter can no longer give, which the user doesn't get.</summary>
    public List<string> DroppedRoleIds { get; set; } = [];
}
