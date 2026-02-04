namespace Serenity.TestUtils;

public class MockPermissions : IPermissionService
{
    public Func<string, bool> HasPermission { get; set; }

    public MockPermissions()
    {
    }

    public MockPermissions(Func<string, bool> hasPermission)
    {
        HasPermission = hasPermission ?? throw new ArgumentNullException(nameof(hasPermission));
    }

    bool IPermissionService.HasPermission(string permission)
    {
        return HasPermission == null || HasPermission(permission);
    }
}