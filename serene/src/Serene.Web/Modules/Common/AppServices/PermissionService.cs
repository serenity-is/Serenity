using Serene.Administration;

namespace Serene.AppServices;

public class PermissionService(ITwoLevelCache cache,
    ISqlConnections sqlConnections,
    ITypeSource typeSource,
    IUserAccessor userAccessor,
    IRolePermissionService rolePermissions,
    IHttpContextItemsAccessor? httpContextItemsAccessor = null,
    IOptions<PermissionServiceOptions>? options = null) :
    BasePermissionService<UserPermissionRow, UserRoleRow>(cache, sqlConnections, typeSource,
        userAccessor, rolePermissions, httpContextItemsAccessor, options)
{
    // the super admins come from the PermissionService:SuperAdmins setting, "admin" by default
}