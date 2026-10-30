namespace Serenity.Testing;

public static partial class TestServiceCollectionExtensions
{
    public static IServiceCollection AddMockPermissions(
        this IServiceCollection collection, Func<string, bool> hasPermission)
    {
        var permissionService = new MockPermissions(hasPermission);
        collection.AddSingleton<IPermissionService>(permissionService);

        return collection;
    }
}