
namespace Serenity.Abstractions;

/// <summary>
/// Retrieves user definitions by identifier or username.
/// </summary>
public interface IUserRetrieveService
{
    /// <summary>
    /// Gets the user definition with the specified identifier.
    /// </summary>
    /// <param name="id">The unique user identifier.</param>
    /// <returns>The matching <see cref="IUserDefinition"/> or <c>null</c> if not found.</returns>
    IUserDefinition? ById(string id);

    /// <summary>
    /// Gets the user definition with the specified username.
    /// </summary>
    /// <param name="username">The username to look up.</param>
    /// <returns>The matching <see cref="IUserDefinition"/> or <c>null</c> if not found.</returns>
    IUserDefinition? ByUsername(string username);
}