namespace Serene.Administration;

[DataScript("Administration.PermissionKeys", Permission = PermissionKeys.Security)]
public class PermissionKeysDataScript : DataScript<IEnumerable<string>>
{
    private readonly IPermissionKeyLister permissionKeyLister;

    public PermissionKeysDataScript(IPermissionKeyLister permissionKeyLister)
    {
        GroupKey = RoleRow.Fields.GenerationKey;
        this.permissionKeyLister = permissionKeyLister ?? throw new ArgumentNullException(nameof(permissionKeyLister));
    }

    protected override IEnumerable<string> GetData()
    {
        return permissionKeyLister.ListPermissionKeys(includeRoles: false);
    }
}