using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serenity.Web;

namespace Serenity.Extensions.DependencyInjection;

/// <summary>
/// DI extension methods related to the HTTP context.
/// </summary>
public static class HttpContextServiceCollectionExtensions
{
    /// <summary>
    /// Adds default implementations for <see cref="IHttpContextAccessor"/> and
    /// <see cref="IHttpContextItemsAccessor"/>.
    /// </summary>
    /// <param name="collection">The service collection to add to.</param>
    /// <returns>The same service collection so that calls can be chained.</returns>
    public static IServiceCollection AddHttpContextItemsAccessor(this IServiceCollection collection)
    {
        collection.AddHttpContextAccessor();
        collection.TryAddSingleton<IHttpContextItemsAccessor, HttpContextItemsAccessor>();
        return collection;
    }

}
