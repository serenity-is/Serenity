namespace Serenity.Extensions;

/// <summary>
/// Why a session of a user ended, see <see cref="AccountSession.EndReason"/>.
/// </summary>
public enum AccountSessionEndReason
{
    /// <summary>The user signed out.</summary>
    SignedOut = 1,

    /// <summary>The user signed it out from another session, e.g. on the my account page.</summary>
    RevokedByUser = 2,

    /// <summary>An administrator signed the user out everywhere.</summary>
    RevokedByAdmin = 3,

    /// <summary>The password of the user changed.</summary>
    PasswordChanged = 4,

    /// <summary>An administrator reset the two-factor authentication of the user.</summary>
    TwoFactorReset = 5,

    /// <summary>The user was deactivated, or no longer exists.</summary>
    Deactivated = 6,

    /// <summary>The user signed in again, and only one session is allowed, see <see cref="AccountSessionConcurrency"/>.</summary>
    Replaced = 7
}
