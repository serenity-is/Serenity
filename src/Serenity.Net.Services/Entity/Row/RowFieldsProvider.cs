using Microsoft.Extensions.DependencyInjection;
using System.Threading;

namespace Serenity.Data;

/// <summary>
/// Contains extension methods and settings for row fields providers
/// </summary>
public static class RowFieldsProvider
{
    private static IRowFieldsProvider defaultProvider;
    private static readonly AsyncLocal<IRowFieldsProvider> localProvider;

    static RowFieldsProvider()
    {
        defaultProvider = FallbackRowFieldsProvider.Instance;
        localProvider = new AsyncLocal<IRowFieldsProvider>();
    }

    /// <summary>
    /// Gets current row fields provider. Returns async local provider if available,
    /// otherwise the default provider.
    /// </summary>
    public static IRowFieldsProvider Current => localProvider.Value ?? defaultProvider;

    /// <summary>
    /// Sets default row fields provider. This instance is required
    /// as rows might have to be created in contexts where dependency injection
    /// is not possible, like deserialization. If using a DI container,
    /// set this at startup to the same singleton service you register with DI.
    /// </summary>
    /// <param name="provider">Provider. Required.</param>
    /// <returns>Old default provider</returns>
    public static IRowFieldsProvider SetDefault(IRowFieldsProvider provider)
    {
        var old = defaultProvider;
        defaultProvider = provider ?? throw new ArgumentNullException(nameof(provider));
        return old;
    }

    /// <summary>
    /// Sets default row fields provider by resolving it from the service provider.
    /// </summary>
    /// <param name="services">Services. Required.</param>
    /// <returns>Old default provider</returns>
    public static IRowFieldsProvider SetDefaultFrom(IServiceProvider services)
    {
        return SetDefault((services ?? throw new ArgumentNullException(nameof(services)))
            .GetRequiredService<IRowFieldsProvider>());
    }

    /// <summary>
    /// Sets local row fields provider for current thread and async context. 
    /// Useful for background tasks, async methods, and testing to set provider locally and for 
    /// auto spawned threads.
    /// </summary>
    /// <param name="provider">Row fields provider. Can be null.</param>
    /// <returns>Old local provider if any.</returns>
    public static IRowFieldsProvider SetLocal(IRowFieldsProvider provider)
    {
        var old = localProvider.Value;
        localProvider.Value = provider;
        return old;
    }

    /// <summary>
    /// Sets local row fields provider by resolving it from the service provider.
    /// </summary>
    /// <param name="services">Services. Required.</param>
    /// <returns>Old default provider</returns>
    public static IRowFieldsProvider SetLocalFrom(IServiceProvider services)
    {
        return SetLocal((services ?? throw new ArgumentNullException(nameof(services)))
            .GetRequiredService<IRowFieldsProvider>());
    }

    /// <summary>
    /// Resolves a fields class using current row fields provider
    /// </summary>
    /// <typeparam name="TFields">The type of the fields.</typeparam>
    /// <returns></returns>
    public static TFields Resolve<TFields>()
        where TFields : RowFieldsBase
    {
        return (TFields)Current.Resolve(typeof(TFields));
    }

    /// <summary>
    /// Resolves an aliased fields class using current row fields provider
    /// </summary>
    /// <typeparam name="TFields">The type of the fields.</typeparam>
    /// <param name="alias">Alias for fields</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">alias</exception>
    public static TFields Resolve<TFields>(string alias)
        where TFields : RowFieldsBase
    {
        if (alias == null)
            throw new ArgumentNullException(nameof(alias));

        return (TFields)Current.ResolveWithAlias(typeof(TFields), alias);
    }
}