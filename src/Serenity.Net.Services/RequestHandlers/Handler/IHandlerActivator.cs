namespace Serenity.Services;

/// <summary>
/// Abstraction for request handler activator, that is used
/// to create instances of an handler type.
/// </summary>
public interface IHandlerActivator
{
    /// <summary>
    /// Creates an instance of the handler type
    /// </summary>
    /// <param name="type">The handler type</param>
    object CreateInstance(Type type);
}