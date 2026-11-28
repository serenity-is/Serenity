namespace Serenity.Extensions;

/// <summary>
/// What an <see cref="IInvitationPolicy"/> decides about an invitation.
/// </summary>
public enum InvitationDecision
{
    /// <summary>The policy has no say, e.g. the inviter is not one it is about.</summary>
    Abstain = 0,

    /// <summary>The policy allows it, unless another one denies it.</summary>
    Allow = 1,

    /// <summary>The policy denies it, whatever the others decide.</summary>
    Deny = 2
}
