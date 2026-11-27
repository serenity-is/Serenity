using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Serenity.Extensions.DependencyInjection;

/// <summary>
/// Contains extensions to register the sessions of the users.
/// </summary>
public static class AccountSessionServiceCollectionExtensions
{
    /// <summary>
    /// Ties the sign-in cookies of the default cookie scheme to sessions (<see cref="AccountSessionCookieValidator"/>), which
    /// the users and the administrators can sign out (<see cref="IAccountSessionManager"/>), and which end when the account
    /// changes for security (<see cref="AccountSessionEventHandler"/>). The sessions are stored by an
    /// <see cref="IAccountSessionStore"/> that the application registers, e.g. the one of Pro.Extensions; without one, nothing
    /// changes. See <see cref="AccountSessionOptions"/>.
    /// </summary>
    /// <param name="collection">The services.</param>
    public static IServiceCollection AddAccountSessions(this IServiceCollection collection)
    {
        ArgumentNullException.ThrowIfNull(collection);
        collection.AddOptions();
        collection.AddHttpContextAccessor();
        collection.AddAccountEvents();
        collection.TryAddSingleton<IAccountSessionManager, AccountSessionManager>();
        collection.TryAddSingleton<AccountSessionCookieValidator>();
        collection.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<CookieAuthenticationOptions>, AccountSessionCookieSetup>());
        collection.TryAddEnumerable(ServiceDescriptor.Singleton<IAccountEventHandler, AccountSessionEventHandler>());
        return collection;
    }
}
