using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Services;

/// <summary>
/// Default implementation of the <see cref="IHandlerActivator"/>
/// </summary>
public class DefaultHandlerActivator : IHandlerActivator
{
    private readonly IServiceProvider provider;

    /// <summary>
    /// Creates an instance of the class
    /// </summary>
    /// <param name="provider">Service provider</param>
    /// <exception cref="ArgumentNullException">provider is null</exception>
    public DefaultHandlerActivator(IServiceProvider provider)
    {
        this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    /// <inheritdoc/>
    public object CreateInstance(Type handlerType)
    {
        return ActivatorUtilities.CreateInstance(provider, handlerType);
    }
}