using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serenity.Web;
using Serenity.Web.EsBuild;

namespace Serenity.Extensions.DependencyInjection;

/// <summary>
/// Extensions for adding EsBuild minifiers to an <see cref="IServiceCollection"/>.
/// </summary>
public static class EsBuildMinifierExtensions
{
    /// <summary>
    /// Adds both the EsBuild CSS and script minifiers to the service collection.
    /// </summary>
    /// <param name="collection">The service collection.</param>
    /// <returns>The same service collection so that calls can be chained.</returns>
    public static IServiceCollection AddEsBuildMinifiers(this IServiceCollection collection)
    {
        AddEsBuildCssMinifier(collection);
        AddEsBuildScriptMinifier(collection);
        return collection;
    }

    /// <summary>
    /// Adds the EsBuild CSS minifier to the service collection.
    /// </summary>
    /// <param name="collection">The service collection.</param>
    /// <returns>The same service collection so that calls can be chained.</returns>
    public static IServiceCollection AddEsBuildCssMinifier(this IServiceCollection collection)
    {
        collection.TryAddSingleton<ICssMinifier, EsBuildMinifier>();
        return collection;
    }

    /// <summary>
    /// Adds the EsBuild script minifier to the service collection.
    /// </summary>
    /// <param name="collection">The service collection.</param>
    /// <returns>The same service collection so that calls can be chained.</returns>
    public static IServiceCollection AddEsBuildScriptMinifier(this IServiceCollection collection)
    {
        collection.TryAddSingleton<IScriptMinifier, EsBuildMinifier>();
        return collection;
    }
}
