using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Services;

/// <summary>
/// Default <see cref="IBehaviorFactory"/> implementation
/// </summary>
/// <remarks>
/// Creates an instance of the class
/// </remarks>
/// <param name="provider">Service provider which will be
/// used to resolve the services that behavior classes might require</param>
/// <exception cref="ArgumentNullException"></exception>
public class DefaultBehaviorFactory(IServiceProvider provider) : IBehaviorFactory
{
    private readonly IServiceProvider provider = provider ?? throw new ArgumentNullException(nameof(provider));

    /// <inheritdoc/>
    public object CreateInstance(Type behaviorType)
    {
        return ActivatorUtilities.CreateInstance(provider, behaviorType);
    }
}