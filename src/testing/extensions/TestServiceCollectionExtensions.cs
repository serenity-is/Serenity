namespace Serenity.Testing;

public static partial class TestServiceCollectionExtensions
{
    public static IServiceCollection AddNullTypeSource(this IServiceCollection collection)
    {
        collection.AddTypeSource([]);
        return collection;
    }

    public static void AddFakeServiceHandlers(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddNullTypeSource();
        services.AddServiceHandlers();
    }
}