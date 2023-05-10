namespace Serenity.Services;

/// <summary>
/// This is an extension for delete behaviors that should be called for exceptions 
/// that occur during delete. It could be useful to preview the exception and
/// raise another exception for FK / PK database errors etc.
/// </summary>
public interface IDeleteExceptionBehavior
{
    /// <summary>Called when an exception occurs during delete</summary>
    /// <param name="handler">Calling delete request handler</param>
    /// <param name="exception">Exception occurred</param>
    void OnException(IDeleteRequestHandler handler, Exception exception);
}