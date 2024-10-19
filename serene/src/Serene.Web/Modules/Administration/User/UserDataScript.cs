namespace Serene.Administration;

/// <summary>
/// This declares a dynamic script with key 'UserData' that will be available from client side.
/// </summary>
[DataScript("UserData", CacheDuration = -1, Permission = "*")]
public class UserDataScript(ITwoLevelCache cache, IPermissionService permissions,
    IPermissionKeyLister permissionKeyLister, IUserProvider userProvider) : DataScript<ScriptUserDefinition>
{
    private readonly ITwoLevelCache cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly IPermissionService permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
    private readonly IPermissionKeyLister permissionKeyLister = permissionKeyLister ?? throw new ArgumentNullException(nameof(permissionKeyLister));
    private readonly IUserProvider userProvider = userProvider ?? throw new ArgumentNullException(nameof(userProvider));

    protected override ScriptUserDefinition GetData()
    {
        var result = new ScriptUserDefinition();

        if (userProvider.GetUserDefinition() is not UserDefinition user)
        {
            result.Permissions = [];
            return result;
        }

        result.Username = user.Username;
        result.DisplayName = user.DisplayName;
        result.IsAdmin = user.Username == "admin";

        result.Permissions = cache.GetLocalStoreOnly("ScriptUserPermissions:" + user.Id, TimeSpan.Zero,
            UserPermissionRow.Fields.GenerationKey, () =>
            {
                var permissions = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

                var permissionsUsedFromScript = cache.GetLocalStoreOnly("PermissionsUsedFromScript",
                    TimeSpan.Zero, RoleRow.Fields.GenerationKey, () =>
                    {
                        return permissionKeyLister.ListPermissionKeys(includeRoles: false)
                            .Where(permissionKey =>
                            {
                                // this sends permission information for all permission keys to client side.
                                // if you don't need all of them to be available from script, filter them here.
                                // this is recommended for security / performance reasons...
                                return true;
                            }).ToArray();
                    });

                foreach (var permissionKey in permissionsUsedFromScript)
                {
                    if (this.permissions.HasPermission(permissionKey))
                        permissions[permissionKey] = true;
                }

                return permissions;
            });

        return result;
    }
}
