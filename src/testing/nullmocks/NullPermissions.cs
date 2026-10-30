namespace Serenity.Testing;

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