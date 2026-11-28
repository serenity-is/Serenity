namespace Serenity.Extensions;

/// <summary>
/// An invitation that someone would send, which the <see cref="IInvitationPolicy"/>s decide about: who is invited, and the
/// roles the user will have when the invitation is accepted.
/// </summary>
public class InvitationDraft
{
    /// <summary>The email address the invitation goes to, which the user signs up with.</summary>
    public string? Email { get; set; }

    /// <summary>The name of the invited person, which the sign-up page fills in.</summary>
    public string? DisplayName { get; set; }

    /// <summary>The IDs of the roles of the application that the user will have, as strings.</summary>
    public List<string> RoleIds { get; set; } = [];
}
