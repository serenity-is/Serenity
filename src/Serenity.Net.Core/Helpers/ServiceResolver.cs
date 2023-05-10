using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Extensions.DependencyInjection;

/// <summary>
/// A generic version of IServiceProvider which resolves a service on demand.
/// </summary>
/// <typeparam name="TService"></typeparam>
public class ServiceResolver<TService> : IServiceResolver<TService> 
    where TService : notnull
{
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="serviceProvider">The service provider</param>
    /// <exception cref="ArgumentNullException">Throws when service provider is null</exception>
    public ServiceResolver(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Resolves TService using the service provider. If the service was registered as transient, this method acts like a factory.
    /// </summary>
    /// <returns>TService instance</returns>
    public TService Resolve()
    {
        return serviceProvider.GetRequiredService<TService>();
    }
}