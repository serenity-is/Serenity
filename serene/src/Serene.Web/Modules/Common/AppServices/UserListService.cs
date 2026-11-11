using MyRow = Serene.Administration.UserRow;

namespace Serene.AppServices;

/// <summary>
/// Lists the users, e.g. for the editors that pick users. It selects only the fields of a user in the list, so the
/// password and other secrets of the users are not loaded.
/// </summary>
public class UserListService(ISqlConnections sqlConnections) : BaseUserListService<MyRow>(sqlConnections)
{
}
