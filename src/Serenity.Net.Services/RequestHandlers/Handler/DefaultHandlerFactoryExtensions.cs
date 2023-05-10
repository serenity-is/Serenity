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
    /// <returns></returns>
    public static THandler CreateHandler<THandler>(this IDefaultHandlerFactory handlerFactory, Type rowType)
    {
        return (THandler)handlerFactory.CreateHandler(rowType, typeof(THandler));
    }
}