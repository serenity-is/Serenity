using MyRow = Serene.Administration.RoleRow;

namespace Serene.AppServices;

/// <summary>
/// Lists the roles, e.g. for the editors that pick roles.
/// </summary>
public class RoleListService(ISqlConnections sqlConnections) : BaseRoleListService<MyRow>(sqlConnections)
{
}
