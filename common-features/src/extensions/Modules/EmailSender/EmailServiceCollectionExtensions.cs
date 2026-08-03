using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Serenity.Extensions.DependencyInjection;

/// <summary>
/// Contains extensions to register email services in Extensions
/// </summary>
public static class EmailServiceCollectionExtensions
{
    /// <summary>
    /// Tries to adds EmailSender as IEmailSender
    /// </summary>
    /// <param name="collection">The service collection.</param>
    public static IServiceCollection AddEmailSender(this IServiceCollection collection)
    {
        collection.TryAddSingleton<IEmailSender, EmailSender>();
        return collection;
    }
}