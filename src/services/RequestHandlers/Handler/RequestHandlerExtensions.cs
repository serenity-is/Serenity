namespace Serenity.Services;

/// <summary>
/// Contains extension methods to create request / response types
/// for a request handler instance
/// </summary>
public static class RequestHandlerExtensions
{
    /// <summary>
    /// Creates a request object for the list handler instance
    /// </summary>
    /// <param name="handler">List handler instance</param>
    public static ListRequest CreateRequest(this IListRequestHandler handler)
    {
        return (ListRequest)Activator.CreateInstance(handler.GetRequestType());
    }

    /// <summary>
    /// Creates a request object for the retrieve handler instance
    /// </summary>
    /// <param name="handler">Retrieve handler instance</param>
    public static RetrieveRequest CreateRequest(this IRetrieveRequestHandler handler)
    {
        return (RetrieveRequest)Activator.CreateInstance(handler.GetRequestType());
    }

    /// <summary>
    /// Creates a request object for the delete handler instance
    /// </summary>
    /// <param name="handler">Delete handler instance</param>
    public static DeleteRequest CreateRequest(this IDeleteRequestHandler handler)
    {
        return (DeleteRequest)Activator.CreateInstance(handler.GetRequestType());
    }

    /// <summary>
    /// Creates a request object for the undelete handler instance
    /// </summary>
    /// <param name="handler">Undelete handler instance</param>
    public static UndeleteRequest CreateRequest(this IUndeleteRequestHandler handler)
    {
        return (UndeleteRequest)Activator.CreateInstance(handler.GetRequestType());
    }

    /// <summary>
    /// Creates a request object for the save handler instance
    /// </summary>
    /// <param name="handler">Save handler instance</param>
    public static SaveRequest<TRow> CreateRequest<TRow>(this ISaveRequestHandler handler)
    {
        return (SaveRequest<TRow>)Activator.CreateInstance(handler.GetRequestType());
    }

    /// <summary>
    /// Creates a request object for the save handler instance
    /// </summary>
    /// <param name="handler">Save handler instance</param>
    public static ISaveRequest CreateRequest(this ISaveRequestHandler handler)
    {
        return (ISaveRequest)Activator.CreateInstance(handler.GetRequestType());
    }

    /// <summary>
    /// Gets the request type for the handler instance
    /// </summary>
    /// <param name="handler">Handler instance</param>
    public static Type GetRequestType(this IRequestHandler handler)
    {
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        return handler.GetType().GetInterfaces()
            .FirstOrDefault(x => x.IsGenericType &&
                x.GetGenericTypeDefinition() == typeof(IRequestType<>))?.GetGenericArguments()[0];
    }

    /// <summary>
    /// Gets the response type for the handler instance
    /// </summary>
    /// <param name="handler">Handler instance</param>
    public static Type GetResponseType(this IRequestHandler handler)
    {
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        return handler.GetType().GetInterfaces()
            .FirstOrDefault(x => x.IsGenericType &&
                x.GetGenericTypeDefinition() == typeof(IResponseType<>))?.GetGenericArguments()[0];
    }
}