
namespace Serenity.Abstractions;

/// <summary>
/// Removes a cached user entry by identifier or username.
/// </summary>
public interface IRemoveCachedUser
{
    /// <summary>
    /// Removes the cached user with the specified identifier and/or username.
    /// </summary>
    /// <param name="userId">The user identifier, or <c>null</c> to skip removal by identifier.</param>
    /// <param name="username">The username, or <c>null</c> to skip removal by username.</param>
    void RemoveCachedUser(string? userId, string? username);
}
