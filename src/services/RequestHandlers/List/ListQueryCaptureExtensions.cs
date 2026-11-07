namespace Serenity.Services;

/// <summary>
/// Extensions to find the query capture capability of list handlers.
/// </summary>
public static class ListQueryCaptureExtensions
{
    /// <summary>
    /// Gets the query capture capability of a list handler, looking through the sync / async
    /// wrappers the handler factory may return, or null if the handler can't capture its query.
    /// A synchronous handler's capture is adapted to the async interface.
    /// </summary>
    /// <param name="handler">List handler, e.g. one returned by the handler factory</param>
    /// <exception cref="ArgumentNullException"><paramref name="handler"/> is null.</exception>
    public static IListQueryCaptureAsync? GetQueryCapture(this IListRequestHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        IListRequestHandler? current = handler;
        while (current is not null)
        {
            if (current is IListQueryCaptureAsync captureAsync)
                return captureAsync;

            if (current is IListQueryCapture capture)
                return new SyncListQueryCaptureAdapter(capture);

            current = (current as IListRequestProcessorWrapper)?.WrappedHandler;
        }

        return null;
    }

    private static readonly HashSet<string> postQueryMethods =
    [
        "ProcessEntity",
        "ExecuteQuery",
        "ExecuteQueryAsync",
        "OnAfterExecuteQuery",
        "OnAfterExecuteQueryAsync",
        "OnReturn",
        "OnReturnAsync"
    ];

    /// <summary>
    /// Gets the names of the methods of a list handler, looking through the sync / async wrappers, that run after
    /// its query is executed and that the handler overrides, e.g. <c>ProcessEntity</c> or <c>OnReturn</c>. As they
    /// don't run when the query is captured, a filter they apply, e.g. by dropping rows, would not take effect, so a
    /// consumer may refuse to capture the query of such a handler unless told that it filters only in its query.
    /// It doesn't check the list behaviors of the handler.
    /// </summary>
    /// <param name="handler">List handler</param>
    /// <returns>The names of the overridden methods, empty if there are none.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="handler"/> is null.</exception>
    public static IReadOnlyList<string> GetPostQueryOverrides(this IListRequestHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        while (handler is IListRequestProcessorWrapper wrapper)
            handler = wrapper.WrappedHandler;

        var serenityAssembly = typeof(IListRequestHandler).Assembly;
        return [.. handler.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(x => postQueryMethods.Contains(x.Name) && x.DeclaringType?.Assembly != serenityAssembly &&
                x.GetBaseDefinition().DeclaringType?.Assembly == serenityAssembly)
            .Select(x => x.Name)
            .Distinct()
            .Order(StringComparer.Ordinal)];
    }
}
