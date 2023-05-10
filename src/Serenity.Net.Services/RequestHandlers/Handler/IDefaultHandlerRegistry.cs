namespace Serenity.Services;

/// <summary>
/// Abstaction for the registry that cobtains default
/// handler types.
/// </summary>
public interface IDefaultHandlerRegistry
{
    /// <summary>
    /// Gets a list of registered handler classes for 
    /// the requested handler interface type.
    /// </summary>
    /// <param name="handlerType">Handler interface type</param>
    IEnumerable<Type> GetTypes(Type handlerType);
}