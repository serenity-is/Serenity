using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Extensions;

/// <summary>
/// Asks the <see cref="IPasswordSignInPolicy"/>s and the <see cref="IPasswordGrantPolicy"/>s whether a user can get a
/// token with the password, e.g. the password grant of a token endpoint.
/// </summary>
public static class PasswordGrantPolicyExtensions
{
    /// <summary>
    /// Gets why a user can't get a token with the password, from the first policy that refuses it, or null when the user
    /// can, after the password is checked. The sign-in policies are asked first, as anything that refuses a sign-in with
    /// the password refuses a token too.
    /// </summary>
    /// <param name="services">The services, with the policies.</param>
    /// <param name="username">The username.</param>
    /// <returns>The message, or null.</returns>
    public static string? GetPasswordGrantDenial(this IServiceProvider services, string username)
    {
        ArgumentNullException.ThrowIfNull(services);
        if (string.IsNullOrEmpty(username))
            return null;

        if (services.GetPasswordSignInDenial(username) is string denial)
            return denial;

        var policies = services.GetServices<IPasswordGrantPolicy>().ToArray();
        if (policies.Length == 0)
            return null;

        var user = services.GetService<IUserRetrieveService>()?.ByUsername(username);
        if (user == null)
            return null;

        foreach (var policy in policies)
            if (policy.GetDenial(user) is string grantDenial)
                return grantDenial;

        return null;
    }
}
