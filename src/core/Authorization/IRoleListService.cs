namespace Serenity.Abstractions;

/// <summary>
/// Lists the roles of the application, e.g. for the editors that pick roles. It only gives the roles, and does not check
/// who is asking, so the services that expose it to the users should have their own permissions.
/// </summary>
public interface IRoleListService
{
    /// <summary>
    /// Lists the roles, ordered by their names.
    /// </summary>
    /// <param name="options">The options, which list all the roles when they are not given.</param>
    /// <returns>The roles.</returns>
    IReadOnlyList<IRoleDefinition> ListRoles(RoleListOptions? options = null);
}
