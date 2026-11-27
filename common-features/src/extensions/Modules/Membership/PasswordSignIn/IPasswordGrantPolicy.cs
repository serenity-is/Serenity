namespace Serenity.Extensions;

/// <summary>
/// Refuses a sign-in with the password that can't ask the user anything more, e.g. the password grant of a token endpoint,
/// which can't ask for a two-factor code or send the user to a page that sets something up. The
/// <see cref="IPasswordSignInPolicy"/>s are asked first, see <see cref="PasswordGrantPolicyExtensions.GetPasswordGrantDenial"/>.
/// Register one with <c>services.TryAddEnumerable(ServiceDescriptor.Singleton&lt;IPasswordGrantPolicy, MyPolicy&gt;())</c>.
/// </summary>
public interface IPasswordGrantPolicy
{
    /// <summary>
    /// Gets why the user can't get a token with the password, or null when the user can.
    /// </summary>
    /// <param name="user">The user, whose password is checked.</param>
    /// <returns>The message, or null.</returns>
    string? GetDenial(IUserDefinition user);
}
