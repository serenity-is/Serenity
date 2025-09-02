namespace Serenity.Services;

/// <summary>
/// This is an extension for list behaviors that should be called for exceptions
/// that occur during list. It could be useful to preview the exception and
/// raise another exception for JSON serialization errors etc.
/// </summary>
public interface IListExceptionBehavior
{
    /// <summary>Called when an exception occurs during list</summary>
    /// <param name="handler">Calling list request handler</param>
    /// <param name="exception">Exception occurred</param>
    void OnException(IListRequestHandler handler, Exception exception);
}