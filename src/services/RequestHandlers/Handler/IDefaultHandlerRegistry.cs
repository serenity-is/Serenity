namespace Serenity.Services;

/// <summary>
/// Abstraction for the registry that contains default
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