using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Serenity.Extensions.DependencyInjection;

/// <summary>
/// Contains extensions to register elevation services in Extensions
/// </summary>
public static class ElevationServiceCollectionExtensions
{
    /// <summary>
    /// Tries to add DefaultElevationHandler as IElevationHandler
    /// </summary>
    /// <param name="collection">The service collection.</param>
    public static IServiceCollection AddElevationHandler(this IServiceCollection collection)
    {
        ArgumentNullException.ThrowIfNull(collection);
        collection.TryAddSingleton<IElevationHandler, DefaultElevationHandler>();
        return collection;
    }
}