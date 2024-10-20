using Serene.Administration;
using System.Security.Claims;

namespace Serene.AppServices;

public class PermissionService(ITwoLevelCache cache,
    ISqlConnections sqlConnections,
    ITypeSource typeSource,
    IUserAccessor userAccessor,
    IRolePermissionService rolePermissions,
    IHttpContextItemsAccessor httpContextItemsAccessor = null) :
    BasePermissionService<UserPermissionRow, UserRoleRow>(cache, sqlConnections, typeSource,
        userAccessor, rolePermissions, httpContextItemsAccessor)
{
    /// <inheritdoc/>
    protected override bool IsSuperAdmin(ClaimsPrincipal user)
    {
        return user.Identity?.Name == "admin";
    }
}