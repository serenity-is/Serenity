
using System.Security.Claims;

namespace Serenity.Extensions;

/// <summary>
/// Base permission service that provides common functionality for permission services.
/// </summary>
/// <typeparam name="TUserPermissionRow">User permission row type</typeparam>
/// <typeparam name="TUserRoleRow">User role row type</typeparam>
/// <param name="cache">Cache</param>
/// <param name="sqlConnections">Sql connections</param>
/// <param name="typeSource">Type source</param>
/// <param name="userAccessor">User accessor</param>
/// <param name="rolePermissions">Role permissions</param>
/// <param name="httpContextItemsAccessor">HTTP context items accessor</param>
public abstract class BasePermissionService<TUserPermissionRow, TUserRoleRow>(
    ITwoLevelCache cache,
    ISqlConnections sqlConnections,
    ITypeSource typeSource,
    IUserAccessor userAccessor,
    IRolePermissionService rolePermissions,
    IHttpContextItemsAccessor httpContextItemsAccessor = null) :
    BasePermissionService(userAccessor, rolePermissions, httpContextItemsAccessor)
    where TUserPermissionRow : class, IUserPermissionRow, new()
    where TUserRoleRow : class, IUserRoleRow, new()
{
    private readonly ITwoLevelCache cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ISqlConnections sqlConnections = sqlConnections ?? throw new ArgumentNullException(nameof(sqlConnections));

    /// <inheritdoc/>
    protected override bool? UserHasPermission(ClaimsPrincipal user, string permission)
    {
        return GetUserPermissions(user)?
            .TryGetValue(permission, out bool grant) == true ? grant : null;
    }

    /// <summary>
    /// Gets directly assigned permissions for the specified user.
    /// </summary>
    /// <param name="user">User</param>
    protected virtual IDictionary<string, bool> GetUserPermissions(ClaimsPrincipal user)
    {
        if (user == null)
            return null;

        return GetCachedUserPermissions(user);
    }

    /// <summary>
    /// Gets the cache duration for user permissions.
    /// Default is zero, meaning it will be cached indefinitely, unless
    /// expired by using the cache group key.
    /// </summary>
    protected virtual TimeSpan GetUserPermissionsCacheDuration()
    {
        return TimeSpan.Zero;
    }

    private TUserPermissionRow userPermissionRow;

    /// <summary>
    /// Gets the cache group key for user permissions.
    /// </summary>
    protected virtual string GetUserPermissionsCacheGroupKey()
    {
        return (userPermissionRow ??= new()).Fields.GenerationKey;
    }

    /// <summary>
    /// Gets the cache key for user permissions.
    /// </summary>
    /// <param name="user">User</param>
    protected virtual string GetUserPermissionsCacheKey(ClaimsPrincipal user)
    {
        return "UserPermissions:" + user.GetIdentifier();
    }

    /// <summary>
    /// Gets the cached user permissions.
    /// </summary>
    /// <param name="user">User</param>
    protected virtual IDictionary<string, bool> GetCachedUserPermissions(ClaimsPrincipal user)
    {
        return cache.GetLocalStoreOnly(GetUserPermissionsCacheKey(user),
            GetUserPermissionsCacheDuration(), GetUserPermissionsCacheGroupKey(), 
            () => LoadUserPermissions(user));
    }

    /// <summary>
    /// Loads user permissions from database.
    /// </summary>
    /// <param name="user">User</param>
    protected virtual IDictionary<string, bool> LoadUserPermissions(ClaimsPrincipal user)
    {
        using var connection = sqlConnections.NewFor<TUserPermissionRow>();
        var result = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        userPermissionRow ??= new();

        connection.List<TUserPermissionRow>(query =>
        {
            var userId = userPermissionRow.UserIdField.ConvertValue(
                user.GetIdentifier(), CultureInfo.InvariantCulture);

            query.Select(userPermissionRow.PermissionKeyField)
                .Where(userPermissionRow.UserIdField == new ValueCriteria(userId));

            if (userPermissionRow.GrantedField is not null)
                query.Select(userPermissionRow.GrantedField);
        }).ForEach(x => result[x.PermissionKeyField[x]] = x.GrantedField?[x] ?? true);

        var implicitPermissions = GetImplicitPermissions(cache.Memory, typeSource);
        foreach (var pair in result.ToArray())
        {
            if (pair.Value && implicitPermissions.TryGetValue(pair.Key, out var list))
                foreach (var x in list)
                    result[x] = true;
        }

        return result;
    }

    /// <summary>
    /// Gets the cache duration for user roles. Default is zero, 
    /// meaning it will be cached indefinitely, unless expired by using 
    /// the cache group key.
    /// </summary>
    protected virtual TimeSpan GetUserRolesCacheDuration()
    {
        return TimeSpan.Zero;
    }

    private TUserRoleRow userRoleRow;

    /// <summary>
    /// Gets the cache group key for user roles.
    /// </summary>
    protected virtual string GetUserRolesCacheGroupKey()
    {
        return (userRoleRow ??= new()).Fields.GenerationKey;
    }

    /// <summary>
    /// Gets the cache key for user roles.
    /// </summary>
    /// <param name="user">User</param>
    protected virtual string GetUserRolesCacheKey(ClaimsPrincipal user)
    {
        return "UserRoles:" + user.GetIdentifier();
    }

    /// <summary>
    /// Gets the cached user roles.
    /// </summary>
    /// <param name="user">User</param>
    protected virtual IEnumerable<string> GetCachedUserRoles(ClaimsPrincipal user)
    {
        return cache.GetLocalStoreOnly(GetUserRolesCacheKey(user),
            GetUserRolesCacheDuration(), GetUserRolesCacheGroupKey(),
            () => LoadUserRoles(user));
    }

    /// <summary>
    /// Gets the roles of the specified user.
    /// </summary>
    /// <param name="user">User</param>
    protected override IEnumerable<string> GetUserRoles(ClaimsPrincipal user)
    {
        if (user == null)
            return null;

        return GetCachedUserRoles(user);
    }

    /// <summary>
    /// Loads user roles from database.
    /// </summary>
    /// <param name="user">User</param>
    protected virtual IEnumerable<string> LoadUserRoles(ClaimsPrincipal user)
    { 
        using var connection = sqlConnections.NewFor<TUserRoleRow>();
        var result = new HashSet<string>();
        userRoleRow ??= new();
        var userId = userRoleRow.UserIdField.ConvertValue(user.GetIdentifier(), 
            CultureInfo.InvariantCulture);

        connection.List<TUserRoleRow>(q => q
            .Select(userRoleRow.RoleKeyOrNameField)
            .Where(userRoleRow.UserIdField == new ValueCriteria(userId)))
                .ForEach(x => result.Add(x.RoleKeyOrNameField[x]));

        return result;
    }
}