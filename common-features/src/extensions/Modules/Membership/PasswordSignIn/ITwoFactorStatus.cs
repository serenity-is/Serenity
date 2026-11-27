namespace Serenity.Extensions;

/// <summary>
/// Tells whether a user has two-factor authentication, for the features that don't depend on its implementation, e.g. a
/// sign-in policy that lets a user with it sign in with the password.
/// </summary>
public interface ITwoFactorStatus
{
    /// <summary>
    /// Returns true if two-factor authentication is active for the given user.
    /// </summary>
    /// <param name="username">Username</param>
    bool IsActiveFor(string username);
}
