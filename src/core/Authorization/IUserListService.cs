namespace Serenity.Abstractions;

/// <summary>
/// Lists the users of the application, e.g. for the editors that pick users, as <see cref="IUserRetrieveService"/>
/// does not know more than the user it is asked for. It only gives the users, and does not check who is asking, so
/// the services that expose it to the users should have their own permissions, and return only what they need.
/// </summary>
public interface IUserListService
{
    /// <summary>
    /// Lists the users, ordered by their usernames.
    /// </summary>
    /// <param name="options">The options, which list all the active users when they are not given.</param>
    /// <returns>The users, which may be limited to some of the properties of a user definition, e.g. without the
    /// password.</returns>
    IReadOnlyList<IUserDefinition> ListUsers(UserListOptions? options = null);
}
