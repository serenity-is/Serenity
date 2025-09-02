namespace Serenity.Services;

/// <summary>
/// Interface to get a new instace of a default request handler,
/// given its row type and the handler interface
/// </summary>
public interface IDefaultHandlerFactory
{
    /// <summary>
    /// Creates a new instance of a default request handler
    /// for the specified row type and the handler interface
    /// </summary>
    /// <param name="rowType"></param>
    /// <param name="handlerInterface"></param>
    /// <returns></returns>
    object CreateHandler(Type rowType, Type handlerInterface);
}