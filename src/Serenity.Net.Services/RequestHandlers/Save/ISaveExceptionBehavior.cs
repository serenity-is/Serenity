namespace Serenity.Services;

/// <summary>
/// This is an extension for save behaviors that should be called for exceptions 
/// that occur during save. It could be useful to preview the exception and
/// raise another exception for FK / PK database errors etc.
/// </summary>
public interface ISaveExceptionBehavior
{
    /// <summary>Called when an exception occurs during save</summary>
    /// <param name="handler">Calling save request handler</param>
    /// <param name="exception">Exception occurred</param>
    void OnException(ISaveRequestHandler handler, Exception exception);
}