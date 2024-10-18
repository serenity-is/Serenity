using Serene.Administration;

namespace Serene.AppServices;

public class RolePermissionService(ITwoLevelCache cache, ISqlConnections sqlConnections,
    ITypeSource typeSource) : IRolePermissionService
{
    private readonly ITwoLevelCache cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ISqlConnections sqlConnections = sqlConnections ?? throw new ArgumentNullException(nameof(sqlConnections));
    private readonly ITypeSource typeSource = typeSource ?? throw new ArgumentNullException(nameof(typeSource));

    public bool HasPermission(string role, string permission)
    {
        return GetRolePermissions(role).Contains(permission);
    }

    private HashSet<string> GetRolePermissions(string role)
    {
        ArgumentNullException.ThrowIfNull(role);

        var fld = RolePermissionRow.Fields;

        return cache.GetLocalStoreOnly("RolePermissions:" + role, TimeSpan.Zero, fld.GenerationKey, () =>
        {
            using var connection = sqlConnections.NewByKey("Default");
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            connection.List<RolePermissionRow>(q => q
                    .Select(fld.PermissionKey)
                    .Where(fld.RoleName == role))
                .ForEach(x => result.Add(x.PermissionKey));

            result.Add("Role:" + role);

            var implicitPermissions = AppServices.PermissionService.GetImplicitPermissions(cache.Memory, typeSource);
            foreach (var key in result.ToArray())
            {
                if (implicitPermissions.TryGetValue(key, out HashSet<string> list))
                    foreach (var x in list)
                        result.Add(x);
            }

            return result;
        });
    }
}