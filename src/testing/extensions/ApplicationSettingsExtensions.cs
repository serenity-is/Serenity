using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Serenity.Testing;

public static partial class TestServiceCollectionExtensions
{
    public static IServiceCollection AddSettings(this IServiceCollection collection, IEnumerable<KeyValuePair<string, string>> settings)
    {
        var builder = new ConfigurationBuilder().AddInMemoryCollection(settings);
        var configuration = builder.Build();
        collection.TryAddSingleton<IConfiguration>(configuration);
        return collection;
    }

    public static IServiceCollection AddSettings(this IServiceCollection collection, string key, string value)
    {
        AddSettings(collection, new Dictionary<string, string>() { { key, value } });
        return collection;
    }

    public static IServiceCollection AddSettings(this IServiceCollection collection,
        params (string Key, string Value)[] settings)
    {
        AddSettings(collection, settings.Select(x => new KeyValuePair<string, string>(x.Key, x.Value)));
        return collection;
    }
}
