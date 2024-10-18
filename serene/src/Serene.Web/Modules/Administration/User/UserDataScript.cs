using Serene.Administration.Repositories;

namespace Serene.Administration;

/// <summary>
/// This declares a dynamic script with key 'UserData' that will be available from client side.
/// </summary>
[DataScript("UserData", CacheDuration = -1, Permission = "*")]
public class UserDataScript : DataScript<ScriptUserDefinition>
{
    private readonly ITwoLevelCache cache;
    private readonly IPermissionService permissions;
    private readonly IPermissionKeyLister permissionKeyLister;
    private readonly IUserAccessor userAccessor;
    private readonly IUserRetrieveService userRetriever;

    public UserDataScript(ITwoLevelCache cache, IPermissionService permissions,
        IPermissionKeyLister permissionKeyLister, IUserAccessor userAccessor, IUserRetrieveService userRetriever)
    {
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        this.permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
        this.permissionKeyLister = permissionKeyLister ?? throw new ArgumentNullException(nameof(permissionKeyLister));
        this.userAccessor = userAccessor ?? throw new ArgumentNullException(nameof(userAccessor));
        this.userRetriever = userRetriever ?? throw new ArgumentNullException(nameof(userRetriever));
    }

    protected override ScriptUserDefinition GetData()
    { 
        var result = new ScriptUserDefinition();

        if (userAccessor.User?.GetUserDefinition(userRetriever) is not UserDefinition user)
        {
            result.Permissions = new Dictionary<string, bool>();
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
