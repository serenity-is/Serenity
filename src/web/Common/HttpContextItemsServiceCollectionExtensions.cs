using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serenity.Web;

namespace Serenity.Extensions.DependencyInjection;

/// <summary>
/// DI extension methods related to HTTPContext
/// </summary>
public static class HttpContextServiceCollectionExtensions
{
    /// <summary>
    /// Add default implementations for IHttpContextAccessor and IHttpContextItemsAccessor
    /// </summary>
    /// <param name="collection">Services</param>
    public static IServiceCollection AddHttpContextItemsAccessor(this IServiceCollection collection)
    {
        collection.AddHttpContextAccessor();
        collection.TryAddSingleton<IHttpContextItemsAccessor, HttpContextItemsAccessor>();
        return collection;
    }

}
