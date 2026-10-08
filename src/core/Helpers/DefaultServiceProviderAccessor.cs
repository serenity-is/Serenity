namespace Serenity;

/// <summary>
/// Default <see cref="IServiceProviderAccessor"/> that always returns the same service provider.
/// </summary>
/// <remarks>
/// Initializes a new instance of the class.
/// </remarks>
/// <param name="serviceProvider">The service provider to return.</param>
/// <exception cref="ArgumentNullException"><paramref name="serviceProvider"/> is <c>null</c>.</exception>
public class DefaultServiceProviderAccessor(IServiceProvider serviceProvider) : IServiceProviderAccessor
{
    /// <inheritdoc/>
    public IServiceProvider ServiceProvider { get; } = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
}
