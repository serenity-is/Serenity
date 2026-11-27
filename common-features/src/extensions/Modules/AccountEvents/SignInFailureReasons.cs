namespace Serenity.Extensions;

/// <summary>
/// Why a sign-in failed, the <see cref="AccountEvent.Reason"/> of <see cref="AccountEventKind.SignInFailed"/>.
/// </summary>
public static class SignInFailureReasons
{
    /// <summary>The password is wrong.</summary>
    public const string InvalidPassword = "InvalidPassword";

    /// <summary>No user has the username, which is not stored.</summary>
    public const string UnknownUser = "UnknownUser";

    /// <summary>The user is not active.</summary>
    public const string InactiveUser = "InactiveUser";

    /// <summary>Too many attempts were made lately.</summary>
    public const string Throttled = "Throttled";

    /// <summary>A sign-in policy refused it, e.g. a user who signs in with an external provider, see
    /// <see cref="IPasswordSignInPolicy"/> and <see cref="IPasswordGrantPolicy"/>.</summary>
    public const string Denied = "Denied";

    /// <summary>The two-factor authentication code is wrong or expired.</summary>
    public const string TwoFactorFailed = "TwoFactorFailed";

    /// <summary>Something else, e.g. the directory could not be reached.</summary>
    public const string Other = "Other";

    /// <summary>
    /// Gets the reason of a password validation result that is not valid.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>The reason.</returns>
    public static string From(PasswordValidationResult result) => result switch
    {
        PasswordValidationResult.Invalid or PasswordValidationResult.EmptyPassword => InvalidPassword,
        PasswordValidationResult.InactiveUser => InactiveUser,
        PasswordValidationResult.Throttle => Throttled,
        _ => Other
    };
}
