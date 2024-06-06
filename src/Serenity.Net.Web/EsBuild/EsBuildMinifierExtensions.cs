using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serenity.Web;
using Serenity.Web.EsBuild;

namespace Serenity.Extensions.DependencyInjection;

/// <summary>
/// Extensions for adding EsBuild minifiers to IServiceCollection.
/// </summary>
public static class EsBuildMinifierExtensions
{
    /// <summary>
    /// Adds EsBuild minifiers to IServiceCollection.
    /// </summary>
    /// <param name="collection">Collection</param>
    public static IServiceCollection AddEsBuildMinifiers(this IServiceCollection collection)
    {
        AddEsBuildCssMinifier(collection);
        AddEsBuildScriptMinifier(collection);
        return collection;
    }

    /// <summary>
    /// Adds EsBuild CSS minifier to IServiceCollection.
    /// </summary>
    /// <param name="collection">Collection</param>
    public static IServiceCollection AddEsBuildCssMinifier(this IServiceCollection collection)
    {
        collection.TryAddSingleton<ICssMinifier, EsBuildMinifier>();
        return collection;
    }

    /// <summary>
    /// Adds EsBuild script minifier to IServiceCollection.
    /// </summary>
    /// <param name="collection">Collection</param>
    public static IServiceCollection AddEsBuildScriptMinifier(this IServiceCollection collection)
    {
        collection.TryAddSingleton<IScriptMinifier, EsBuildMinifier>();
        return collection;
    }
}
