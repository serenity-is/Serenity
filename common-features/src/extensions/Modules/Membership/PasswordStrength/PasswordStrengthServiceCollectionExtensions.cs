using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Serenity.Extensions.DependencyInjection;

/// <summary>
/// Contains extensions to register password streng validator in Extensions
/// </summary>
public static class PasswordStrengthServiceCollectionExtensions
{
    /// <summary>
    /// Tries to add PasswordStrengthValidator as IPasswordStrengValidator
    /// </summary>
    /// <param name="collection">The service collection.</param>
    public static IServiceCollection AddPasswordStrengthValidator(this IServiceCollection collection)
    {
        collection.TryAddSingleton<IPasswordStrengthValidator, PasswordStrengthValidator>();
        return collection;
    }
}