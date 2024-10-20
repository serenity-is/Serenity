
namespace Serenity.Extensions;

/// <summary>
/// Base permission service that provides common functionality for permission services.
/// </summary>
/// <param name="userAccessor">User accessor</param>
/// <param name="rolePermissions">Role permission service</param>
/// <param name="httpContextItemsAccessor">HTTP context items accessor</param>
public abstract class BaseRolePermissionService<TRolePermissionRow>(
    ITwoLevelCache cache, ISqlConnections sqlConnections, ITypeSource typeSource) : IRolePermissionService
    where TRolePermissionRow : class, IRolePermissionRow, new()
{
    private readonly ITwoLevelCache cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ISqlConnections sqlConnections = sqlConnections ?? throw new ArgumentNullException(nameof(sqlConnections));
    private readonly ITypeSource typeSource = typeSource ?? throw new ArgumentNullException(nameof(typeSource)); 

    /// <summary>
    /// Gets whether the specified role is a valid role key or name.
    /// By default, a role is valid if it is not null or empty.
    /// </summary>
    /// <param name="key">Role key</param>
    protected virtual bool IsValidRoleKeyOrName(string role)
    {
        return !string.IsNullOrEmpty(role);
    }

    private TRolePermissionRow rolePermissionRow;

    /// <inheritdoc/>
    public bool HasPermission(string role, string permission)
    {
        if (!IsValidRoleKeyOrName(role))
            return false;

        return GetCachedRolePermissions(role).Contains(permission);
    }

    /// <summary>
    /// Gets the cache key for role permissions.
    /// </summary>
    /// <param name="role">Role</param>
    protected virtual string GetCacheKey(string role)
    {
        return "RolePermissions:" + role;
    }

    /// <summary>
    /// Gets the cache group key for role permissions.
    /// </summary>
    protected virtual string GetCacheGroupKey()
    {
        return (rolePermissionRow ??= new()).Fields.GenerationKey;
    }

    /// <summary>
    /// Gets the cache duration for role permissions. Default is zero, meaning it will be cached indefinitely, 
    /// unless expired by using the cache group key.
    /// </summary>
    protected virtual TimeSpan GetCacheDuration()
    {
        return TimeSpan.Zero;
    }

    /// <summary>
    /// Gets the role permissions for the specified role.
    /// </summary>
    /// <param name="role">Role</param>
    protected virtual ISet<string> GetCachedRolePermissions(string role)
    {
        return cache.GetLocalStoreOnly(GetCacheKey(role), GetCacheDuration(), GetCacheGroupKey(),
            () => LoadRolePermissions(role));
    }

    /// <summary>
    /// Loads role permissions from database.
    /// </summary>
    /// <param name="role">Role key or name</param>
    protected virtual ISet<string> LoadRolePermissions(string role)
    {
        using var connection = sqlConnections.NewFor<TRolePermissionRow>();
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        rolePermissionRow ??= new();
        connection.List<TRolePermissionRow>(q => q
            .Select(rolePermissionRow.PermissionKeyField)
            .Where(rolePermissionRow.RoleKeyOrNameField == role))
            .ForEach(x => result.Add(x.PermissionKeyField[x]));

        result.Add("Role:" + role);

        var implicitPermissions = BasePermissionService.GetImplicitPermissions(cache.Memory, typeSource);
        foreach (var key in result.ToArray())
        {
            if (implicitPermissions.TryGetValue(key, out HashSet<string> list))
                foreach (var x in list)
                    result.Add(x);
        }

        return result;
    }
}
