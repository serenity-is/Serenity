namespace Serenity.Extensions;

/// <summary>
/// An invitation to sign up, see <see cref="IInvitationManager"/>. The link of the invitation has a token, which is stored
/// only hashed, so the invitations can't be accepted from the stored ones.
/// </summary>
public class Invitation
{
    /// <summary>The ID of the invitation.</summary>
    public long InvitationId { get; set; }

    /// <summary>The email address the invitation went to, which the user signs up with.</summary>
    public string? Email { get; set; }

    /// <summary>The name of the invited person, which the sign-up page fills in.</summary>
    public string? DisplayName { get; set; }

    /// <summary>The IDs of the roles of the application that the user will have, as strings.</summary>
    public List<string> RoleIds { get; set; } = [];

    /// <summary>The ID of the user who invited.</summary>
    public string? InvitedByUserId { get; set; }

    /// <summary>The username of the user who invited, when it invited.</summary>
    public string? InvitedByUsername { get; set; }

    /// <summary>The display name of the user who invited, when it invited.</summary>
    public string? InvitedByDisplayName { get; set; }

    /// <summary>When it was created, in UTC.</summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>When it was last sent, in UTC: a resend sends a new link, and the old one no longer works.</summary>
    public DateTime SentDate { get; set; }

    /// <summary>When the link of the last sending expires, in UTC.</summary>
    public DateTime ExpiresDate { get; set; }

    /// <summary>When the user signed up with it, in UTC.</summary>
    public DateTime? AcceptedDate { get; set; }

    /// <summary>The ID of the user who signed up with it.</summary>
    public string? AcceptedUserId { get; set; }

    /// <summary>When it was revoked, in UTC.</summary>
    public DateTime? RevokedDate { get; set; }

    /// <summary>
    /// Gets the state of the invitation at a time.
    /// </summary>
    /// <param name="now">The time, in UTC.</param>
    public InvitationStatus GetStatus(DateTime now) =>
        AcceptedDate != null ? InvitationStatus.Accepted :
        RevokedDate != null ? InvitationStatus.Revoked :
        ExpiresDate <= now ? InvitationStatus.Expired :
        InvitationStatus.Pending;
}
