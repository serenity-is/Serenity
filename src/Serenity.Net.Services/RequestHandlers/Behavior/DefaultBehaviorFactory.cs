using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Services;

/// <summary>
/// Default <see cref="IBehaviorFactory"/> implementation
/// </summary>
public class DefaultBehaviorFactory : IBehaviorFactory
{
    private readonly IServiceProvider provider;

    /// <summary>
    /// Creates an instance of the class
    /// </summary>
    /// <param name="provider">Service provider which will be
    /// used to resolve the services that behavior classes might require</param>
    /// <exception cref="ArgumentNullException"></exception>
    public DefaultBehaviorFactory(IServiceProvider provider)
    {
        this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    /// <inheritdoc/>
    public object CreateInstance(Type behaviorType)
    {
        return ActivatorUtilities.CreateInstance(provider, behaviorType);
    }
}