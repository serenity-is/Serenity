
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;
using System.Security.Claims;

namespace Serenity.Extensions;

/// <summary>
/// Base permission service that provides common functionality for permission services.
/// </summary>
/// <param name="userAccessor">User accessor</param>
/// <param name="rolePermissions">Role permission service</param>
/// <param name="httpContextItemsAccessor">HTTP context items accessor</param>
public abstract class BasePermissionService(
    IUserAccessor userAccessor,
    IRolePermissionService rolePermissions,
    IHttpContextItemsAccessor httpContextItemsAccessor = null) : IPermissionService, ITransientGrantor
{
    private readonly IRolePermissionService rolePermissions = rolePermissions ?? throw new ArgumentNullException(nameof(rolePermissions));
    private readonly IUserAccessor userAccessor = userAccessor ?? throw new ArgumentNullException(nameof(userAccessor));
    private readonly TransientGrantingPermissionService transientGrantor = new(permissionService: null, httpContextItemsAccessor);

    /// <summary>
    /// Gets whether the specified permission is a valid permission key.
    /// By default, a permission key is valid if it is not null or empty.
    /// </summary>
    /// <param name="permission">Permission</param>
    protected virtual bool IsValidKey(string permission)
    {
        return !string.IsNullOrEmpty(permission);
    }

    /// <summary>
    /// Gets whether the specified permission is an asterisk, e.g. "*" that
    /// grants permission to all including anonymous users.
    /// </summary>
    /// <param name="permission">Permission</param>
    protected virtual bool IsAsterisk(string permission)
    {
        return permission == "*";
    }

    /// <summary>
    /// Gets whether the specified permission is a question mark, e.g. "?" that
    /// grants permission to logged in users.
    /// </summary>
    /// <param name="permission">Permission</param>
    protected virtual bool IsQuestionMark(string permission)
    {
        return permission == "?";
    }

    /// <summary>
    /// Checks if the transient grantor has the specified permission.
    /// </summary>
    /// <param name="permission">Permission</param>
    protected virtual bool IsTransientlyGranted(string permission)
    {
        return transientGrantor.HasPermission(permission);
    }

    /// <summary>
    /// Checks if anonymous users have the specified permission.
    /// By default, they don't have any permission.
    /// </summary>
    /// <param name="permission"></param>
    /// <returns></returns>
    protected virtual bool AnonymousUsersHavePermission(string permission)
    {
        return false;
    }

    /// <summary>
    /// Returns true if the provided user has the permission to impersonate as another user.
    /// Unless overridden, no user have this permission.
    /// </summary>
    /// <param name="user">User</param>
    /// <param name="permission">Permission</param>
    protected virtual bool HasImpersonationPermission(ClaimsPrincipal user, string permission)
    {
        return IsSuperAdmin(user);
    }

    /// <summary>
    /// Gets whether the specified permission is an impersonation permission.
    /// By default, impersonation permissions are permissions that start with "ImpersonateAs".
    /// </summary>
    /// <param name="permission"></param>
    /// <returns></returns>
    protected virtual bool IsImpersonationPermission(string permission)
    {
        return permission.StartsWith("ImpersonateAs", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets whether the specified user is a super admin.
    /// </summary>
    /// <param name="username">Username</param>
    protected virtual bool IsSuperAdmin(ClaimsPrincipal user)
    {
        return false;
    }

    /// <summary>
    /// Checks if the super admin has the specified permission.
    /// By default, super admin has all permissions.
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="permission">Permission</param>
    /// <returns></returns>
    protected virtual bool SuperAdminHasPermission(ClaimsPrincipal user, string permission)
    {
        return true;
    }

    /// <inheritdoc/>
    public virtual bool HasPermission(string permission)
    {
        if (!IsValidKey(permission))
            return false;

        if (IsAsterisk(permission) || IsTransientlyGranted(permission))
            return true;

        var isLoggedIn = userAccessor.IsLoggedIn();

        if (IsQuestionMark(permission))
            return isLoggedIn;

        if (!isLoggedIn)
            return AnonymousUsersHavePermission(permission);

        var user = userAccessor.User;
        if (string.IsNullOrEmpty(user?.Identity?.Name))
            return AnonymousUsersHavePermission(permission);

        if (IsImpersonationPermission(permission))
            return HasImpersonationPermission(user, permission);

        if (IsSuperAdmin(user) &&
            SuperAdminHasPermission(user, permission))
            return true;

        var userPermission = UserHasPermission(user, permission);
        if (userPermission != null)
            return userPermission.Value;

        foreach (var role in GetUserRoles(user))
        {
            if (rolePermissions.HasPermission(role, permission))
                return true;
        }

        return false;
    }


    /// <summary>
    /// Gets the roles of the specified user.
    /// </summary>
    /// <param name="user">User</param>
    protected abstract IEnumerable<string> GetUserRoles(ClaimsPrincipal user);

    /// <summary>
    /// Gets if user has the specified permission directly, not via roles.
    /// Returns null if permission is not granted or denied directly.
    /// </summary>
    /// <param name="user">User</param>
    /// <param name="permission">Permission</param>
    protected abstract bool? UserHasPermission(ClaimsPrincipal user, string permission);

    /// <summary>
    /// Gets implicit permissions defined in the application.
    /// </summary>
    /// <param name="memoryCache">Memory cache</param>
    /// <param name="typeSource">Type source</param>
    /// <returns></returns>
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
    public virtual void Grant(params string[] permissions)
    {
        transientGrantor.Grant(permissions);
    }

    /// <inheritdoc/>
    public virtual void GrantAll()
    {
        transientGrantor.GrantAll();
    }

    /// <inheritdoc/>
    public virtual void UndoGrant()
    {
        transientGrantor.UndoGrant();
    }

    /// <inheritdoc/>
    public virtual bool IsAllGranted() => transientGrantor.IsAllGranted();

    /// <inheritdoc/>
    public virtual IEnumerable<string> GetGranted() => transientGrantor.GetGranted();
}