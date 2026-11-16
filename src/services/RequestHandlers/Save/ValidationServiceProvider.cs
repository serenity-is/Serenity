namespace Serenity.Services;

/// <summary>
/// The services of the <see cref="System.ComponentModel.DataAnnotations.ValidationContext"/> a validation rule is checked in
/// by a request handler: its own ones first, e.g. the connection in its transaction and the localizer of the request, then
/// the ones of the request, when the request context gives them, see <see cref="IServiceProviderAccessor"/>.
/// </summary>
/// <param name="services">The services of the handler, by their types.</param>
/// <param name="fallback">The services of the request, or null when there are none.</param>
public class ValidationServiceProvider(IReadOnlyDictionary<Type, object?> services, IServiceProvider? fallback = null) : IServiceProvider
{
    private readonly IReadOnlyDictionary<Type, object?> services = services ?? throw new ArgumentNullException(nameof(services));

    /// <inheritdoc/>
    public object? GetService(Type serviceType)
    {
        if (serviceType is null)
            throw new ArgumentNullException(nameof(serviceType));

        if (services.TryGetValue(serviceType, out var service) && service is not null)
            return service;

        return fallback?.GetService(serviceType);
    }
}
