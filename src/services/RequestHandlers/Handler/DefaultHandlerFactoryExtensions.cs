namespace Serenity.Services;

/// <summary>
/// Extension methods for <see cref="IDefaultHandlerFactory"/>
/// </summary>
public static class DefaultHandlerFactoryExtensions
{
    /// <summary>
    /// Creates an instance of the default handler for
    /// the requested handler interface type.
    /// </summary>
    /// <typeparam name="THandler">Handler interface type</typeparam>
    /// <param name="handlerFactory">Default handler factory</param>
    /// <param name="rowType">Row type</param>
    /// <returns>The created handler instance.</returns>
    public static THandler CreateHandler<THandler>(this IDefaultHandlerFactory handlerFactory, Type rowType)
    {
        return (THandler)handlerFactory.CreateHandler(rowType, typeof(THandler));
    }

    /// <summary>
    /// Creates the default handler for <paramref name="rowType"/> as
    /// <typeparamref name="TCreated"/> and casts it to <typeparamref name="TExpected"/>,
    /// throwing a descriptive error if the created handler does not implement it.
    /// </summary>
    /// <remarks>
    /// Used by the generic request handler proxies (see
    /// <see cref="ServiceCollectionExtensions.AddProxyRequestHandlers"/>) to adapt the processor
    /// interface they resolve from <see cref="IDefaultHandlerFactory"/> to the generic handler
    /// interface the proxy exposes.
    /// </remarks>
    /// <typeparam name="TCreated">The handler interface the factory is asked to create.</typeparam>
    /// <typeparam name="TExpected">The handler interface the caller expects.</typeparam>
    /// <param name="handlerFactory">Default handler factory</param>
    /// <param name="rowType">Row type</param>
    /// <returns>The created handler instance.</returns>
    internal static TExpected CreateHandlerForProxy<TCreated, TExpected>(this IDefaultHandlerFactory handlerFactory, Type rowType)
        where TExpected : class
    {
        var created = handlerFactory.CreateHandler<TCreated>(rowType);
        return created as TExpected ?? throw new InvalidProgramException(
            $"The handler for row type {rowType.FullName} does not implement " +
            $"{typeof(TExpected).FullName}. Custom handlers must derive from the " +
            "corresponding default request handler base class.");
    }
}