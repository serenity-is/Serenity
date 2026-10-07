namespace Serenity.Services;

/// <summary>
/// Generic handler proxy that resolves the retrieve handler for <typeparamref name="TRow"/> through
/// <see cref="IDefaultHandlerFactory"/> and forwards calls to it.
/// </summary>
/// <remarks>
/// See <see cref="Serenity.Extensions.DependencyInjection.ServiceCollectionExtensions.AddProxyRequestHandlers"/> for why these exist.
/// </remarks>
internal class RetrieveHandlerProxy<TRow, TRetrieveRequest, TRetrieveResponse>
    : IRetrieveHandler<TRow, TRetrieveRequest, TRetrieveResponse>
    where TRow : class, IRow, IIdRow, new()
    where TRetrieveRequest : RetrieveRequest
    where TRetrieveResponse : RetrieveResponse<TRow>, new()
{
    private readonly IRetrieveHandler<TRow, TRetrieveRequest, TRetrieveResponse> handler;

    public RetrieveHandlerProxy(IDefaultHandlerFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        handler = factory.CreateHandlerForProxy<IRetrieveRequestProcessor, IRetrieveHandler<TRow, TRetrieveRequest, TRetrieveResponse>>(typeof(TRow));
    }

    public TRetrieveResponse Retrieve(IDbConnection connection, TRetrieveRequest request)
    {
        return handler.Retrieve(connection, request);
    }
}

internal class RetrieveHandlerProxy<TRow>(IDefaultHandlerFactory factory)
    : RetrieveHandlerProxy<TRow, RetrieveRequest, RetrieveResponse<TRow>>(factory), IRetrieveHandler<TRow>
    where TRow : class, IRow, IIdRow, new()
{
}