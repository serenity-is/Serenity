namespace Serenity.Services;

/// <summary>
/// This is an extension for undelete behaviors that should be called for exceptions 
/// that occur during delete. It could be useful to preview the exception and
/// raise another exception for FK / PK database errors etc.
/// </summary>
public interface IUndeleteExceptionBehavior
{
    /// <summary>Called when an exception occurs during undelete</summary>
    /// <param name="handler">Calling undelete request handler</param>
    /// <param name="exception">Exception occurred</param>
    void OnException(IUndeleteRequestHandler handler, Exception exception);
}