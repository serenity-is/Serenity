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
}
