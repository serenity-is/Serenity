namespace Serenity.Services;

/// <summary>
/// This is an extension for retrieve behaviors that should be called for exceptions
/// that occur during retrieve. It could be useful to preview the exception and
/// raise another exception for JSON serialization errors etc.
/// </summary>
public interface IRetrieveExceptionBehavior
{
    /// <summary>Called when an exception occurs during retrieve</summary>
    /// <param name="handler">Calling retrieve request handler</param>
    /// <param name="exception">Exception occurred</param>
    void OnException(IRetrieveRequestHandler handler, Exception exception);
}