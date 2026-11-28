namespace Serenity.Extensions;

/// <summary>
/// Who can sign up, see <see cref="MembershipSettings.SignUp"/>.
/// </summary>
public enum SignUpMode
{
    /// <summary>Anyone can sign up, with a password or an external login.</summary>
    Open = 0,

    /// <summary>Only the ones with an invitation can sign up, see <see cref="IInvitationManager"/>.</summary>
    InvitationOnly = 1,

    /// <summary>Nobody can sign up; the administrators add the users.</summary>
    Closed = 2
}
