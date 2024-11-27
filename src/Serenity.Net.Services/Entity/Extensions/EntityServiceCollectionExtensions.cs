using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Serenity.Extensions.DependencyInjection;

/// <summary>
/// Contains extensions to register entity services
/// </summary>
public static class EntityServiceCollectionExtensions
{
    /// <summary>
    /// Adds the annotation types, sql connections, default row type registry and
    /// row fields provider. 
    /// Warning: This method sets the singleton row fields provider as
    /// the default by calling RowFieldsProvider.SetDefault on creation.
    /// </summary>
    /// <param name="services">The services.</param>
    public static void AddEntities(this IServiceCollection services)
    {
        services.AddSqlConnections();
        services.AddAnnotationTypes();
        services.TryAddSingleton<IRowTypeRegistry, DefaultRowTypeRegistry>();
        services.TryAddSingleton<IRowFieldsProvider, DefaultRowFieldsProvider>();
    }
}