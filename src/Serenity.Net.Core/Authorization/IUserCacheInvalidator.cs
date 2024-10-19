
namespace Serenity.Abstractions;

/// <summary>
/// Abstraction to remove a cached user by its id or username
/// </summary>
public interface IUserCacheInvalidator
{
    /// <summary>
    /// Removes all cached users
    /// </summary>
    void InvalidateAll();

    /// <summary>
    /// Invalidates cached user by its user definition
    /// </summary>
    void InvalidateItem(IUserDefinition? userDefinition);

    /// <summary>
    /// Removes cached user by user ID
    /// </summary>
    void InvalidateById(string? userId);

    /// <summary>
    /// Removes cached user by username
    /// </summary>
    void InvalidateByUsername(string? username);
}
