using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Serenity.Testing;

public static partial class TestServiceCollectionExtensions
{
    public static IServiceCollection AddMockUser(
        this IServiceCollection collection, Func<string> getUsername,
        Func<string> getIdentifier = null,
        Func<string, bool> hasPermission = null)
    {
        var userAccessor = new MockUserAccessor(getUsername, getIdentifier);
        collection.TryAddSingleton<IUserAccessor>(userAccessor);
        if (hasPermission != null)
            collection.AddMockPermissions(hasPermission);
        return collection;
    }

    public static IServiceCollection UseSysAdmin(
        this IServiceCollection collection)
    {
        return AddMockUser(collection, () => TestUser.SysAdmin, hasPermission: perm => true);
    }

    public static IServiceCollection UseGuest(
        this IServiceCollection collection, Func<string, bool> hasPermission = null)
    {
        return AddMockUser(collection, () => TestUser.Guest, hasPermission: hasPermission ?? (perm => false));
    }

    public static IServiceCollection AddMockImpersonatingUserAccessor(
        this IServiceCollection collection, Func<string> getUsername)
    {
        var mockContextItems = new MockHttpContextItemsAccessor();
        var mockUserAccessor = new MockUserAccessor(getUsername);

        collection.AddSingleton<IHttpContextItemsAccessor>(mockContextItems);
        collection.AddSingleton<IUserAccessor>(new ImpersonatingUserAccessor(mockUserAccessor, mockContextItems));

        return collection;
    }

    public static IServiceCollection AddMockUserRetrieveService(
        this IServiceCollection collection, Func<string, IUserDefinition> getUserDefinition)
    {
        ArgumentNullException.ThrowIfNull(getUserDefinition);

        collection.AddSingleton<IUserRetrieveService>(new FuncMockUserRetrieveService(getUserDefinition));
        return collection;
    }
}