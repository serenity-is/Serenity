using Microsoft.Extensions.Caching.Memory;

namespace Serenity.Extensions;

/// <summary>
/// The default <see cref="IPermissionGrantValidator"/>: a permission can be granted by the tier of the
/// <see cref="PermissionGrantOptions"/> that it and each permission it implies have; a super admin can grant anything. The
/// account of a user can be changed by the user, by a super admin, or by someone who could grant all the permissions that
/// <see cref="IUserGrantedPermissionsProvider"/> gives for the user, but not by anyone else for a super admin. Only the
/// permissions that the <see cref="IPermissionKeyLister"/> lists can be added, when there is one.
/// </summary>
/// <param name="permissions">The permission service, for the permissions of the current user.</param>
/// <param name="userAccessor">Gives the current user.</param>
/// <param name="grantOptions">The rules of who can grant what.</param>
/// <param name="serviceOptions">The options of the permission service, with the super admins.</param>
/// <param name="memoryCache">The cache of the implicit permissions.</param>
/// <param name="typeSource">The types, for the implicit permissions.</param>
/// <param name="localizer">The localizer of the messages.</param>
/// <param name="grantedPermissions">Gives the permissions of a user, without which only super admins are protected.</param>
/// <param name="keyLister">Lists the permissions that can be added, without which any key can.</param>
/// <param name="elevation">Checks that the user confirmed the password lately, for the sensitive permissions, without
/// which it is not asked for.</param>
public class PermissionGrantValidator(IPermissionService permissions,
    IUserAccessor userAccessor,
    IOptions<PermissionGrantOptions> grantOptions,
    IOptions<PermissionServiceOptions> serviceOptions,
    IMemoryCache memoryCache,
    ITypeSource typeSource,
    ITextLocalizer localizer,
    IUserGrantedPermissionsProvider? grantedPermissions = null,
    IPermissionKeyLister? keyLister = null,
    IElevationHandler? elevation = null) : IPermissionGrantValidator
{
    private readonly IPermissionService permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
    private readonly IUserAccessor userAccessor = userAccessor ?? throw new ArgumentNullException(nameof(userAccessor));

    /// <inheritdoc/>
    public bool IsSuperAdmin => serviceOptions?.Value?.IsSuperAdmin(userAccessor.User?.Identity?.Name) == true;

    private PermissionGrantDenial? GetOwnDenial(string permissionKey)
    {
        var tier = (grantOptions?.Value ?? new PermissionGrantOptions()).GetTier(permissionKey);
        if (tier == PermissionGrantTier.SuperAdminOnly)
            return PermissionGrantDenial.SuperAdminOnly;

        if (tier == PermissionGrantTier.HolderOnly && !permissions.HasPermission(permissionKey))
            return PermissionGrantDenial.NotHeld;

        return null;
    }

    private PermissionGrantTier GetTier(string permissionKey) =>
        (grantOptions?.Value ?? new PermissionGrantOptions()).GetTier(permissionKey);

    /// <inheritdoc/>
    public bool IsSensitive(string permissionKey)
    {
        if (string.IsNullOrEmpty(permissionKey))
            return false;

        if (GetTier(permissionKey) != PermissionGrantTier.Grantable)
            return true;

        var implicitPermissions = BasePermissionService.GetImplicitPermissions(memoryCache, typeSource);
        return implicitPermissions.TryGetValue(permissionKey, out var implied) &&
            implied.Any(x => GetTier(x) != PermissionGrantTier.Grantable);
    }

    /// <inheritdoc/>
    public PermissionGrantDenial? GetDenial(string permissionKey)
    {
        if (string.IsNullOrEmpty(permissionKey) || IsSuperAdmin)
            return null;

        var denial = GetOwnDenial(permissionKey);
        if (denial != null)
            return denial;

        // a permission brings the ones that it implies, one level deep, as the permission service does
        var implicitPermissions = BasePermissionService.GetImplicitPermissions(memoryCache, typeSource);
        if (implicitPermissions.TryGetValue(permissionKey, out var implied))
            foreach (var key in implied)
                if ((denial = GetOwnDenial(key)) != null)
                    return denial;

        return null;
    }

    /// <inheritdoc/>
    public void ValidateGrants(IEnumerable<string> permissionKeys)
    {
        var keys = (permissionKeys ?? []).ToList();
        if (!IsSuperAdmin)
        {
            foreach (var key in keys)
            {
                var denial = GetDenial(key);
                if (denial != null)
                    throw new ValidationError("PermissionGrantDenied", key, string.Format(CultureInfo.CurrentCulture,
                        (denial == PermissionGrantDenial.SuperAdminOnly ? PermissionGrantTexts.SuperAdminOnly : PermissionGrantTexts.NotHeld)
                            .ToString(localizer), key));
            }
        }

        // the sensitive ones need the password confirmed lately, also for a super admin, after the denials, so someone
        // who can't grant them is not asked for it
        if (elevation != null && keys.Any(IsSensitive))
            elevation.ValidateElevationToken();
    }

    /// <inheritdoc/>
    public void ValidateListed(IEnumerable<string> permissionKeys)
    {
        if (keyLister == null || permissionKeys == null)
            return;

        var listed = keyLister.ListPermissionKeys(includeRoles: false);
        var set = listed as ISet<string> ?? new HashSet<string>(listed, StringComparer.OrdinalIgnoreCase);
        foreach (var key in permissionKeys)
            if (!set.Contains(key ?? ""))
                throw new ValidationError("PermissionNotListed", key, string.Format(CultureInfo.CurrentCulture,
                    PermissionGrantTexts.NotListed.ToString(localizer), key));
    }

    private bool IsCurrentUser(IUserDefinition user)
    {
        var current = userAccessor.User;
        if (current?.Identity?.IsAuthenticated != true)
            return false;

        var currentId = current.GetIdentifier();
        return (!string.IsNullOrEmpty(currentId) && string.Equals(currentId, user.Id, StringComparison.OrdinalIgnoreCase)) ||
            string.Equals(current.Identity.Name, user.Username, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc/>
    public PermissionGrantDenial? GetUserDenial(IUserDefinition user)
    {
        ArgumentNullException.ThrowIfNull(user);

        if (IsSuperAdmin || IsCurrentUser(user))
            return null;

        if (serviceOptions?.Value?.IsSuperAdmin(user.Username) == true)
            return PermissionGrantDenial.SuperAdminOnly;

        foreach (var key in grantedPermissions?.GetGrantedPermissions(user.Id) ?? [])
        {
            var denial = GetDenial(key);
            if (denial != null)
                return denial;
        }

        return null;
    }

    /// <inheritdoc/>
    public void ValidateUser(IUserDefinition user)
    {
        var denial = GetUserDenial(user);
        if (denial != null)
            throw new ValidationError("PermissionGrantDenied", null, string.Format(CultureInfo.CurrentCulture,
                (denial == PermissionGrantDenial.SuperAdminOnly ? PermissionGrantTexts.UserSuperAdmin : PermissionGrantTexts.UserNotHeld)
                    .ToString(localizer), user.DisplayName ?? user.Username));
    }
}
