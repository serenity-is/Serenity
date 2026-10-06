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
        return InstantiateRequest<ListRequest>(handler);
    }

    /// <summary>
    /// Sets whether paging limits (<see cref="RequestHandlerSettings.DefaultPageSize"/> /
    /// <see cref="RequestHandlerSettings.MaxPageSize"/>) should be ignored for the list request.
    /// Server side code that needs all rows (e.g. exports, reports, background jobs, nested
    /// relation loads) should call this; otherwise enabling the limits may silently truncate
    /// the results. Has no effect on JSON bound client requests.
    /// </summary>
    /// <typeparam name="TRequest">List request type.</typeparam>
    /// <param name="request">The list request.</param>
    /// <param name="suppress">True to ignore paging limits, false to apply them.</param>
    /// <returns>The same request, for chaining.</returns>
    public static TRequest SuppressPagingLimits<TRequest>(this TRequest request, bool suppress = true)
        where TRequest : ListRequest
    {
        ArgumentNullException.ThrowIfNull(request);
        request.pagingLimitsSuppressed = suppress;
        return request;
    }

    /// <summary>
    /// Gets whether paging limits (<see cref="RequestHandlerSettings.DefaultPageSize"/> /
    /// <see cref="RequestHandlerSettings.MaxPageSize"/>) are suppressed for the list request,
    /// e.g. set via <see cref="SuppressPagingLimits{TRequest}"/>.
    /// </summary>
    /// <typeparam name="TRequest">List request type.</typeparam>
    /// <param name="request">The list request.</param>
    /// <returns>True if paging limits should be ignored.</returns>
    public static bool IsPagingLimitsSuppressed<TRequest>(this TRequest request)
        where TRequest : ListRequest
    {
        ArgumentNullException.ThrowIfNull(request);
        return request.pagingLimitsSuppressed;
    }

    /// <summary>
    /// Creates a request object for the retrieve handler instance
    /// </summary>
    /// <param name="handler">Retrieve handler instance</param>
    public static RetrieveRequest CreateRequest(this IRetrieveRequestHandler handler)
    {
        return InstantiateRequest<RetrieveRequest>(handler);
    }

    /// <summary>
    /// Creates a request object for the delete handler instance
    /// </summary>
    /// <param name="handler">Delete handler instance</param>
    public static DeleteRequest CreateRequest(this IDeleteRequestHandler handler)
    {
        return InstantiateRequest<DeleteRequest>(handler);
    }

    /// <summary>
    /// Creates a request object for the undelete handler instance
    /// </summary>
    /// <param name="handler">Undelete handler instance</param>
    public static UndeleteRequest CreateRequest(this IUndeleteRequestHandler handler)
    {
        return InstantiateRequest<UndeleteRequest>(handler);
    }

    /// <summary>
    /// Creates a request object for the save handler instance
    /// </summary>
    /// <param name="handler">Save handler instance</param>
    public static SaveRequest<TRow> CreateRequest<TRow>(this ISaveRequestHandler handler)
    {
        return InstantiateRequest<SaveRequest<TRow>>(handler);
    }

    /// <summary>
    /// Creates a request object for the save handler instance
    /// </summary>
    /// <param name="handler">Save handler instance</param>
    public static ISaveRequest CreateRequest(this ISaveRequestHandler handler)
    {
        return InstantiateRequest<ISaveRequest>(handler);
    }

    internal static T InstantiateRequest<T>(IRequestHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        var type = handler.GetRequestType()
            ?? throw new InvalidOperationException(
                $"The handler {handler.GetType().Name} does not declare a request type (IRequestType<{typeof(T).Name}>).");

        return Activator.CreateInstance(type) is T request
            ? request
            : throw new InvalidOperationException(
                $"The request type {type.Name} of handler {handler.GetType().Name} cannot be assigned to {typeof(T).Name}.");
    }

    /// <summary>
    /// Gets the request type for the handler instance
    /// </summary>
    /// <param name="handler">Handler instance</param>
    public static Type? GetRequestType(this IRequestHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        return handler.GetType().GetInterfaces()
            .FirstOrDefault(x => x.IsGenericType &&
                x.GetGenericTypeDefinition() == typeof(IRequestType<>))?.GetGenericArguments()[0];
    }

    /// <summary>
    /// Gets the response type for the handler instance
    /// </summary>
    /// <param name="handler">Handler instance</param>
    public static Type? GetResponseType(this IRequestHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        return handler.GetType().GetInterfaces()
            .FirstOrDefault(x => x.IsGenericType &&
                x.GetGenericTypeDefinition() == typeof(IResponseType<>))?.GetGenericArguments()[0];
    }
}