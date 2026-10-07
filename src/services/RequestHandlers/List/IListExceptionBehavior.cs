namespace Serenity.Services;

/// <summary>
/// This is an extension for list behaviors that should be called for exceptions
/// that occur during list. It could be useful to preview the exception and
/// raise another exception for JSON serialization errors etc.
/// </summary>
/// <remarks>
/// All behaviors implementing this interface are called in order when a list fails. A behavior
/// may translate the original exception by throwing a new one; dispatch then stops and that new
/// exception is what propagates, so the first behavior that throws wins and later behaviors never
/// see the original exception. If no behavior throws, the original exception is rethrown unchanged.
/// </remarks>
public interface IListExceptionBehavior
{
    /// <summary>Called when an exception occurs during list</summary>
    /// <param name="handler">Calling list request handler</param>
    /// <param name="exception">Exception occurred</param>
    void OnException(IListRequestHandler handler, Exception exception);
}