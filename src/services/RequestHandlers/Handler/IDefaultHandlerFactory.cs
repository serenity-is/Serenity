namespace Serenity.Services;

/// <summary>
/// Interface to get a new instance of a default request handler,
/// given its row type and the handler interface.
/// </summary>
public interface IDefaultHandlerFactory
{
    /// <summary>
    /// Creates a new instance of a default request handler
    /// for the specified row type and the handler interface.
    /// </summary>
    /// <param name="rowType">The row type.</param>
    /// <param name="handlerInterface">The handler interface type.</param>
    /// <returns>The created handler instance.</returns>
    object CreateHandler(Type rowType, Type handlerInterface);
}