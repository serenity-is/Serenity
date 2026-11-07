namespace Serenity.Services;

/// <summary>
/// Exposes a synchronous <see cref="IListQueryCapture"/> as an <see cref="IListQueryCaptureAsync"/>.
/// </summary>
/// <param name="capture">Synchronous capture</param>
internal sealed class SyncListQueryCaptureAdapter(IListQueryCapture capture) : IListQueryCaptureAsync
{
    public Task<SqlQuery> CaptureQueryAsync(IDbConnection connection, ListRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(capture.CaptureQuery(connection, request));
    }
}
