namespace Serenity.Services;

internal class RetrieveHandlerProxyAsync<TRow, TRetrieveRequest, TRetrieveResponse>
    : IRetrieveHandlerAsync<TRow, TRetrieveRequest, TRetrieveResponse>
    where TRow : class, IRow, IIdRow, new()
    where TRetrieveRequest : RetrieveRequest
    where TRetrieveResponse : RetrieveResponse<TRow>, new()
{
    private readonly IRetrieveHandlerAsync<TRow, TRetrieveRequest, TRetrieveResponse> handler;

    public RetrieveHandlerProxyAsync(IDefaultHandlerFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        handler = (IRetrieveHandlerAsync<TRow, TRetrieveRequest, TRetrieveResponse>) factory.CreateHandler<IRetrieveRequestProcessorAsync>(typeof(TRow));
    }

    public Task<TRetrieveResponse> RetrieveAsync(IDbConnection connection, TRetrieveRequest request, CancellationToken cancellationToken = default)
    {
        return handler.RetrieveAsync(connection, request, cancellationToken);
    }
}

internal class RetrieveHandlerProxyAsync<TRow>(IDefaultHandlerFactory factory)
    : RetrieveHandlerProxyAsync<TRow, RetrieveRequest, RetrieveResponse<TRow>>(factory), IRetrieveHandlerAsync<TRow>
    where TRow : class, IRow, IIdRow, new()
{
}
