namespace Serenity.Testing;

public class MockRolePermissionService(Func<string, string, bool> hasPermission) : IRolePermissionService
{
    private readonly Func<string, string, bool> hasPermission = hasPermission ?? throw new ArgumentNullException(nameof(hasPermission));

    public bool HasPermission(string role, string permission)
    {
        return hasPermission(role, permission);
    }
}