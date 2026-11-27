using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Serenity.Extensions.DependencyInjection;

/// <summary>
/// Contains extensions to register what users must do before using the application.
/// </summary>
public static class AccountSetupServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="AccountSetupFilter"/> to the global filters, which keeps a user who must still set up the account to
    /// the page that does it, and <see cref="PasswordChangeRequirement"/>, which requires the users whose definitions say so
    /// to change their password. Other requirements, e.g. the one of two-factor authentication, are added as
    /// <see cref="IAccountSetupRequirement"/>s. It can be called more than once.
    /// </summary>
    /// <param name="collection">The service collection.</param>
    public static IServiceCollection AddAccountSetup(this IServiceCollection collection)
    {
        ArgumentNullException.ThrowIfNull(collection);
        collection.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<MvcOptions>, AccountSetupMvcOptionsSetup>());
        collection.TryAddEnumerable(ServiceDescriptor.Singleton<IAccountSetupRequirement, PasswordChangeRequirement>());
        // a token with the password can't be used to do them
        collection.TryAddEnumerable(ServiceDescriptor.Singleton<IPasswordGrantPolicy, AccountSetupPasswordGrantPolicy>());
        return collection;
    }
}
