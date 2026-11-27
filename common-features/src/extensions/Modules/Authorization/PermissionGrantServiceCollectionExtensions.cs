using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Serenity.Extensions.DependencyInjection;

/// <summary>
/// Contains extensions to register the checks of who can grant which permissions.
/// </summary>
public static class PermissionGrantServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="PermissionGrantValidator"/> as the <see cref="IPermissionGrantValidator"/>, with the
    /// <see cref="PermissionGrantOptions"/> that <c>ConfigureSections</c> reads from the "PermissionGrants" section of the
    /// settings. The application also registers its <see cref="IUserGrantedPermissionsProvider"/>, without which only
    /// super admins are protected from administrators changing their accounts.
    /// </summary>
    /// <param name="collection">The service collection.</param>
    public static IServiceCollection AddPermissionGrants(this IServiceCollection collection)
    {
        ArgumentNullException.ThrowIfNull(collection);
        collection.TryAddSingleton<IPermissionGrantValidator, PermissionGrantValidator>();
        return collection;
    }
}
