namespace Serenity.TestUtils;

public class NullPermissions : IPermissionService
{
    public NullPermissions()
    {
    }

    public bool HasPermission(string permission)
    {
        return false;
    }
}