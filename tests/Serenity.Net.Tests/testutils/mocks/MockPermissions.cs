namespace Serenity.Tests;

public class MockPermissions : IPermissionService
{
    private readonly Func<string, bool> hasPermission;

    public MockPermissions()
    {
    }

    public MockPermissions(Func<string, bool> hasPermission)
    {
        this.hasPermission = hasPermission ?? throw new ArgumentNullException(nameof(hasPermission));
    }

    public bool HasPermission(string permission)
    {
        return hasPermission == null || hasPermission(permission);
    }
}