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
    TwoFactorReset = 6
}
