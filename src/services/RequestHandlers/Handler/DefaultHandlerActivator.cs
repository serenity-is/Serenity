using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Services;

/// <summary>
/// Default implementation of the <see cref="IHandlerActivator"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the class.
/// </remarks>
/// <param name="serviceProviderAccessor">Accessor for the service provider used to resolve the services handler classes require</param>
/// <exception cref="ArgumentNullException"><paramref name="serviceProviderAccessor"/> is <c>null</c>.</exception>
public class DefaultHandlerActivator(IServiceProviderAccessor serviceProviderAccessor) : IHandlerActivator
{
    private readonly IServiceProviderAccessor serviceProviderAccessor = serviceProviderAccessor ?? throw new ArgumentNullException(nameof(serviceProviderAccessor));

    /// <inheritdoc/>
    public object CreateInstance(Type handlerType)
    {
        return ActivatorUtilities.CreateInstance(serviceProviderAccessor.ServiceProvider, handlerType);
    }
}