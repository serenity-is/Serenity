using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Services;

/// <summary>
/// Default <see cref="IBehaviorFactory"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the class.
/// </remarks>
/// <param name="serviceProviderAccessor">Accessor for the service provider which will be
/// used to resolve the services that behavior classes might require</param>
/// <exception cref="ArgumentNullException"><paramref name="serviceProviderAccessor"/> is <c>null</c>.</exception>
public class DefaultBehaviorFactory(IServiceProviderAccessor serviceProviderAccessor) : IBehaviorFactory
{
    private readonly IServiceProviderAccessor serviceProviderAccessor = serviceProviderAccessor ?? throw new ArgumentNullException(nameof(serviceProviderAccessor));

    /// <inheritdoc/>
    public object CreateInstance(Type behaviorType)
    {
        return ActivatorUtilities.CreateInstance(serviceProviderAccessor.ServiceProvider, behaviorType);
    }
}