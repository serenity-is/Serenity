namespace Serenity.Services;

/// <summary>
/// Interface for behaviors that wrap another behavior to adapt it between
/// synchronous and asynchronous variants. Exposes the inner behavior so
/// framework code can check whether it implements additional interfaces
/// (e.g. <see cref="ISaveExceptionBehavior"/>).
/// </summary>
public interface IWrappedBehavior
{
    /// <summary>
    /// Gets the wrapped (inner) behavior.
    /// </summary>
    object WrappedBehavior { get; }
}
