namespace Serenity.Abstractions;

/// <summary>
/// Interface for getting user ClaimsPrincipal
/// </summary>
public interface IUserClaimCreator
{
    /// <summary>
    /// Gets the ClaimsPrincipal for user with given username
    /// </summary>
    /// <param name="username"></param>
    /// <param name="authType"></param>
    /// <returns></returns>
    ClaimsPrincipal CreatePrincipal(string username, string authType);
}