namespace Serenity.Extensions;

/// <summary>
/// The state of an <see cref="Invitation"/>.
/// </summary>
public enum InvitationStatus
{
    /// <summary>It was sent, and can be accepted until it expires.</summary>
    Pending = 1,

    /// <summary>The user signed up with it.</summary>
    Accepted = 2,

    /// <summary>It was revoked before it was accepted.</summary>
    Revoked = 3,

    /// <summary>It was not accepted in time.</summary>
    Expired = 4
}
