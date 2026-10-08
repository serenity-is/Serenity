using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Extensions.DependencyInjection;

/// <summary>
/// A generic version of IServiceProvider which resolves a service on demand.
/// </summary>
/// <typeparam name="TService">The type of the service to resolve.</typeparam>
/// <remarks>
/// Initializes a new instance.
/// </remarks>
/// <param name="serviceProviderAccessor">Accessor for the service provider to resolve the service on demand.</param>
/// <exception cref="ArgumentNullException">Throws when the service provider accessor is null.</exception>
public class ServiceResolver<TService>(IServiceProviderAccessor serviceProviderAccessor) : IServiceResolver<TService> 
    where TService : notnull
{
    private readonly IServiceProviderAccessor serviceProviderAccessor = serviceProviderAccessor ?? throw new ArgumentNullException(nameof(serviceProviderAccessor));

    /// <summary>
    /// Resolves TService using the service provider. If the service was registered as transient, this method acts like a factory.
    /// </summary>
    /// <returns>TService instance.</returns>
    public TService Resolve()
    {
        return serviceProviderAccessor.ServiceProvider.GetRequiredService<TService>();
    }
}