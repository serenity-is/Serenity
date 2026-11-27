namespace Serenity.Extensions;

/// <summary>
/// What happened to an account, see <see cref="AccountEvent"/>.
/// </summary>
public enum AccountEventKind
{
    /// <summary>The password was changed, by the user, an administrator, or a reset link.</summary>
    PasswordChanged = 1,

    /// <summary>The email address was changed, from <see cref="AccountEvent.OldValue"/> to <see cref="AccountEvent.NewValue"/>.</summary>
    EmailChanged = 2,

    /// <summary>The username was changed, from <see cref="AccountEvent.OldValue"/> to <see cref="AccountEvent.NewValue"/>.</summary>
    UsernameChanged = 3,

    /// <summary>A two-factor authentication method was set up, see <see cref="AccountEvent.Method"/>.</summary>
    TwoFactorEnabled = 4,

    /// <summary>A two-factor authentication method was removed, see <see cref="AccountEvent.Method"/>.</summary>
    TwoFactorDisabled = 5,

    /// <summary>All the two-factor authentication methods were removed by an administrator, e.g. for a lost device.</summary>
    TwoFactorReset = 6,

    /// <summary>The user signed in, with <see cref="AccountEvent.Method"/>, e.g. "Password", "Google", or "Token" for the
    /// password grant of the token endpoint.</summary>
    SignedIn = 20,

    /// <summary>A sign-in failed, for <see cref="AccountEvent.Reason"/>, see <see cref="SignInFailureReasons"/>. Without a
    /// user when the username is not known, which is not stored, as it may be a password typed in the wrong box.</summary>
    SignInFailed = 21,

    /// <summary>The user signed out.</summary>
    SignedOut = 22,

    /// <summary>The user confirmed the password for the sensitive actions, see <see cref="IElevationHandler"/>.</summary>
    Elevated = 23,

    /// <summary>The user entered a wrong password to confirm a sensitive action.</summary>
    ElevationFailed = 24,

    /// <summary>A link to reset the password was emailed to the user.</summary>
    PasswordResetRequested = 25
}
