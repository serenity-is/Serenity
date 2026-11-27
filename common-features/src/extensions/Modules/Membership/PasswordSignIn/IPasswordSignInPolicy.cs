namespace Serenity.Extensions;

/// <summary>
/// Decides whether a user can sign in with the password of the application, which the sign-in pages and services ask after
/// the password is checked, e.g. one that refuses the users who are linked to an external provider, as the password would
/// get around the sign-in of the provider. See <see cref="PasswordSignInPolicyExtensions.GetPasswordSignInDenial"/>.
/// </summary>
public interface IPasswordSignInPolicy
{
    /// <summary>
    /// Gets why the user can't sign in with the password, or null when the user can.
    /// </summary>
    /// <param name="user">The user, whose password is checked.</param>
    /// <returns>The message, or null.</returns>
    string? GetDenial(IUserDefinition user);
}
