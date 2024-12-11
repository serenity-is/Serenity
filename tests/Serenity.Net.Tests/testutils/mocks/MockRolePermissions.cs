namespace Serenity.TestUtils;

public class MockRolePermissions : IRolePermissionService
{
    public Func<string, bool> HasPermissionCallback { get; set; } = perm => false;

    public MockRolePermissions()
    {
    }

    public MockRolePermissions(Func<string, bool> hasPermission)
    {
        this.HasPermissionCallback = hasPermission ?? 
            throw new ArgumentNullException(nameof(hasPermission));
    }

    public bool HasPermission(string role, string permission)
    {
        return HasPermissionCallback?.Invoke(permission) ?? false;
    }
}