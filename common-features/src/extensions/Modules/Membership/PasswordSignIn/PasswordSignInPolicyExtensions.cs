using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Extensions;

/// <summary>
/// Asks the <see cref="IPasswordSignInPolicy"/>s whether a user can sign in with the password.
/// </summary>
public static class PasswordSignInPolicyExtensions
{
    /// <summary>The error code of a sign-in with the password that a policy refuses.</summary>
    public const string PasswordSignInDeniedCode = "PasswordSignInDenied";

    /// <summary>
    /// Gets why a user can't sign in with the password, from the first policy that refuses it, or null when the user can,
    /// e.g. after the password is checked.
    /// </summary>
    /// <param name="services">The services, with the policies.</param>
    /// <param name="username">The username.</param>
    /// <returns>The message, or null.</returns>
    public static string? GetPasswordSignInDenial(this IServiceProvider services, string username)
    {
        ArgumentNullException.ThrowIfNull(services);
        var policies = services.GetServices<IPasswordSignInPolicy>().ToArray();
        if (policies.Length == 0 || string.IsNullOrEmpty(username))
            return null;

        var user = services.GetService<IUserRetrieveService>()?.ByUsername(username);
        if (user == null)
            return null;

        foreach (var policy in policies)
            if (policy.GetDenial(user) is string denial)
                return denial;

        return null;
    }

    /// <summary>
    /// Throws a validation error with the <see cref="PasswordSignInDeniedCode"/> code when a user can't sign in with the
    /// password, see <see cref="GetPasswordSignInDenial"/>.
    /// </summary>
    /// <param name="services">The services, with the policies.</param>
    /// <param name="username">The username.</param>
    public static void ValidatePasswordSignIn(this IServiceProvider services, string username)
    {
        if (services.GetPasswordSignInDenial(username) is string denial)
            throw new ValidationError(PasswordSignInDeniedCode, denial);
    }
}
