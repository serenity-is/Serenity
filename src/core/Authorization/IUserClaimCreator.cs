namespace Serenity.Abstractions;

/// <summary>
/// Creates a <see cref="ClaimsPrincipal"/> for a given username.
/// </summary>
public interface IUserClaimCreator
{
    /// <summary>
    /// Creates a principal for the specified user.
    /// </summary>
    /// <param name="username">The username of the user to create the principal for.</param>
    /// <param name="authType">The authentication type to assign to the created identity.</param>
    /// <returns>The created <see cref="ClaimsPrincipal"/>.</returns>
    ClaimsPrincipal CreatePrincipal(string username, string authType);
}