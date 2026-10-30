namespace Serenity.Testing;

public static partial class TestServiceCollectionExtensions
{
    public static IServiceCollection AddAuditableServices(this IServiceCollection collection,
        params Type[] additionalTypes)
    {
        collection.AddSingleton<ITypeSource>(new MockTypeSource(new[] {
            typeof(UpdateInsertLogBehavior)
        }.Concat(additionalTypes).ToArray()));

        collection.AddServiceHandlers();

        return collection;
    }
}