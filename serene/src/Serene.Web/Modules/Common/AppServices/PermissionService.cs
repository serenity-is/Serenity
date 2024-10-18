using Microsoft.Extensions.Caching.Memory;
using Serene.Administration;
using System.Globalization;
using System.Reflection;

namespace Serene.AppServices;

public class PermissionService(ITwoLevelCache cache,
    ISqlConnections sqlConnections,
    ITypeSource typeSource,
    IUserAccessor userAccessor,
    IRolePermissionService rolePermissions,
    IHttpContextItemsAccessor httpContextItemsAccessor = null) : IPermissionService, ITransientGrantor
{
    private readonly ITwoLevelCache cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly IRolePermissionService rolePermissions = rolePermissions ?? throw new ArgumentNullException(nameof(rolePermissions));
    private readonly ISqlConnections sqlConnections = sqlConnections ?? throw new ArgumentNullException(nameof(sqlConnections));
    private readonly ITypeSource typeSource = typeSource ?? throw new ArgumentNullException(nameof(typeSource));
    private readonly IUserAccessor userAccessor = userAccessor ?? throw new ArgumentNullException(nameof(userAccessor));
    private readonly TransientGrantingPermissionService transientGrantor = new(permissionService: null, httpContextItemsAccessor);

    public bool HasPermission(string permission)
    {
        if (string.IsNullOrEmpty(permission))
            return false;

        if (permission == "*" || transientGrantor.HasPermission(permission))
            return true;

        var isLoggedIn = userAccessor.IsLoggedIn();

        if (permission == "?")
            return isLoggedIn;

        if (!isLoggedIn)
            return false;

        var username = userAccessor.User?.Identity?.Name;
        if (username == "admin")
            return true;

        // only admin has impersonation permission
        if (string.Compare(permission, "ImpersonateAs", StringComparison.OrdinalIgnoreCase) == 0)
            return false;

        var userId = Convert.ToInt32(userAccessor.User.GetIdentifier(), CultureInfo.InvariantCulture);

        if (GetUserPermissions(userId).TryGetValue(permission, out bool grant))
            return grant;

        foreach (var role in GetUserRoles(userId))
        {
            if (rolePermissions.HasPermission(role, permission))
                return true;
        }

        return false;
    }

    private Dictionary<string, bool> GetUserPermissions(int userId)
    {
        var fld = UserPermissionRow.Fields;

        return cache.GetLocalStoreOnly("UserPermissions:" + userId, TimeSpan.Zero, fld.GenerationKey, () =>
        {
            using var connection = sqlConnections.NewByKey("Default");
            var result = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

            connection.List<UserPermissionRow>(q => q
                    .Select(fld.PermissionKey)
                    .Select(fld.Granted)
                    .Where(new Criteria(fld.UserId) == userId))
                .ForEach(x => result[x.PermissionKey] = x.Granted ?? true);

            var implicitPermissions = PermissionService.GetImplicitPermissions(cache.Memory, typeSource);
            foreach (var pair in result.ToArray())
            {
                if (pair.Value && implicitPermissions.TryGetValue(pair.Key, out HashSet<string> list))
                    foreach (var x in list)
                        result[x] = true;
            }

            return result;
        });
    }

    private HashSet<string> GetUserRoles(int userId)
    {
        var fld = UserRoleRow.Fields;

        return cache.GetLocalStoreOnly("UserRoles:" + userId, TimeSpan.Zero, fld.GenerationKey, () =>
        {
            using var connection = sqlConnections.NewByKey("Default");
            var result = new HashSet<string>();

            connection.List<UserRoleRow>(q => q
                    .Select(fld.RoleName)
                    .Where(new Criteria(fld.UserId) == userId))
                .ForEach(x => result.Add(x.RoleName));

            return result;
        });
    }

    public static IDictionary<string, HashSet<string>> GetImplicitPermissions(IMemoryCache memoryCache,
        ITypeSource typeSource)
    {
        ArgumentNullException.ThrowIfNull(memoryCache);

        ArgumentNullException.ThrowIfNull(typeSource);

        return memoryCache.Get<IDictionary<string, HashSet<string>>>("ImplicitPermissions", TimeSpan.Zero, () =>
        {
            var result = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

            void addFrom(Type type)
            {
                foreach (var member in type.GetFields(BindingFlags.Static | BindingFlags.DeclaredOnly |
                    BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (member.FieldType != typeof(string))
                        continue;

                    if (member.GetValue(null) is not string key)
                        continue;

                    foreach (var attr in member.GetCustomAttributes<ImplicitPermissionAttribute>())
                    {
                        if (!result.TryGetValue(key, out HashSet<string> list))
                        {
                            list = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                            result[key] = list;
                        }

                        list.Add(attr.Value);
                    }
                }

                foreach (var nested in type.GetNestedTypes(BindingFlags.Public | BindingFlags.DeclaredOnly))
                    addFrom(nested);
            }

            foreach (var type in typeSource.GetTypesWithAttribute(
                typeof(NestedPermissionKeysAttribute)))
            {
                addFrom(type);
            }

            return result;
        });
    }

    /// <inheritdoc/>
    public void Grant(params string[] permissions)
    {
        transientGrantor.Grant(permissions);
    }

    /// <inheritdoc/>
    public void GrantAll()
    {
        transientGrantor.GrantAll();
    }

    /// <inheritdoc/>
    public void UndoGrant()
    {
        transientGrantor.UndoGrant();
    }

    /// <inheritdoc/>
    public bool IsAllGranted() => transientGrantor.IsAllGranted();

    /// <inheritdoc/>
    public IEnumerable<string> GetGranted() => transientGrantor.GetGranted();

}