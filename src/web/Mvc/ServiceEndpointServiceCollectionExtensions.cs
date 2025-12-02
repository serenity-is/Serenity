using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Serenity.Extensions.DependencyInjection;

/// <summary>
/// DI extension methods related to service endpoints
/// </summary>
public static class ServiceEndpointServiceCollectionExtensions
{
    /// <summary>
    /// Adds service endpoint related services and conventions to the service collection
    /// </summary>
    /// <param name="services"></param>
    public static void AddServiceEndpointConventions(this IServiceCollection services)
    {
        services.TryAddEnumerable(ServiceDescriptor.Transient<IApplicationModelProvider, ServiceEndpointApplicationModelProvider>());

        ArgumentNullException.ThrowIfNull(services);
        services.Configure<Microsoft.AspNetCore.Mvc.MvcOptions>(options =>
        {
            if (!options.ModelMetadataDetailsProviders.OfType<ServiceEndpointBindingMetadataProvider>().Any())
                options.ModelMetadataDetailsProviders.Add(new ServiceEndpointBindingMetadataProvider());
        });
    }

}
