
namespace Serenity.Abstractions;

/// <summary>
/// Abstraction to remove a cached user by its id or username
/// </summary>
public interface IRemoveCachedUser
{
    /// <summary>
    /// Removed cached user by its user ID and/or username
    /// </summary>
    void RemoveCachedUser(string? userId, string? username);
}
