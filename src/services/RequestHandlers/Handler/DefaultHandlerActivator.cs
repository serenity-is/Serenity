using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Services;

/// <summary>
/// Default implementation of the <see cref="IHandlerActivator"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the class.
/// </remarks>
/// <param name="provider">Service provider</param>
/// <exception cref="ArgumentNullException"><paramref name="provider"/> is <c>null</c>.</exception>
public class DefaultHandlerActivator(IServiceProvider provider) : IHandlerActivator
{
    private readonly IServiceProvider provider = provider ?? throw new ArgumentNullException(nameof(provider));

    /// <inheritdoc/>
    public object CreateInstance(Type handlerType)
    {
        return ActivatorUtilities.CreateInstance(provider, handlerType);
    }
}