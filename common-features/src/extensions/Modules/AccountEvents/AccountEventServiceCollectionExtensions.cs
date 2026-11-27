using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Serenity.Extensions.DependencyInjection;

/// <summary>
/// Contains extensions to register the events of the accounts.
/// </summary>
public static class AccountEventServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="AccountEventPublisher"/> as the <see cref="IAccountEventPublisher"/>, and
    /// <see cref="AccountEventEmailHandler"/>, which emails the users when their account changes, e.g. the password, with the
    /// <see cref="AccountEventEmailOptions"/> that <c>ConfigureSections</c> reads. Other handlers can be added as
    /// <see cref="IAccountEventHandler"/>s.
    /// </summary>
    /// <param name="collection">The service collection.</param>
    public static IServiceCollection AddAccountEvents(this IServiceCollection collection)
    {
        ArgumentNullException.ThrowIfNull(collection);
        collection.TryAddSingleton<IAccountEventPublisher, AccountEventPublisher>();
        collection.TryAddEnumerable(ServiceDescriptor.Singleton<IAccountEventHandler, AccountEventEmailHandler>());
        return collection;
    }
}
