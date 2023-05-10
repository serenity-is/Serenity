
namespace Serenity.Abstractions;

/// <summary>
/// Abstraction to retrieve a user definition by its id or username
/// </summary>
public interface IUserRetrieveService
{
    /// <summary>
    /// Returns user definition for specified user ID or null if doesn't exist.
    /// </summary>
    IUserDefinition ById(string id);
    /// <summary>
    /// Returns user definition for specified username or null if doesn't exist.
    /// </summary>
    IUserDefinition ByUsername(string username);
}