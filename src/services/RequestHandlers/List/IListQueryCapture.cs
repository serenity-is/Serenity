namespace Serenity.Services;

/// <summary>
/// Implemented by list handlers that can build the query they would execute for a
/// request without executing it, to use it as a source of another query.
/// Use <see cref="ListQueryCaptureExtensions.GetQueryCapture"/> to find this capability
/// through a list request processor, which may be a sync / async wrapper.
/// </summary>
public interface IListQueryCapture
{
    /// <summary>
    /// Builds the query the handler would execute for the request, without executing it.
    /// See <see cref="ListRequestHandler{TRow, TListRequest, TListResponse}.CaptureQuery"/>.
    /// </summary>
    /// <param name="connection">Connection</param>
    /// <param name="request">List request</param>
    SqlQuery CaptureQuery(IDbConnection connection, ListRequest request);
}
