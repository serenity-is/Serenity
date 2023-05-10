namespace Serenity.Services;

internal class RetrieveHandlerProxy<TRow, TRetrieveRequest, TRetrieveResponse>
    : IRetrieveHandler<TRow, TRetrieveRequest, TRetrieveResponse>
    where TRow : class, IRow, IIdRow, new()
    where TRetrieveRequest : RetrieveRequest
    where TRetrieveResponse : RetrieveResponse<TRow>, new()
{
    private readonly IRetrieveHandler<TRow, TRetrieveRequest, TRetrieveResponse> handler;

    public RetrieveHandlerProxy(IDefaultHandlerFactory factory)
    {
        if (factory is null)
            throw new ArgumentNullException(nameof(factory));

        handler = (IRetrieveHandler<TRow, TRetrieveRequest, TRetrieveResponse>) factory.CreateHandler<IRetrieveRequestProcessor>(typeof(TRow));
    }

    public TRetrieveResponse Retrieve(IDbConnection connection, TRetrieveRequest request)
    {
        return handler.Retrieve(connection, request);
    }
}

internal class RetrieveHandlerProxy<TRow>
    : RetrieveHandlerProxy<TRow, RetrieveRequest, RetrieveResponse<TRow>>, IRetrieveHandler<TRow>
    where TRow : class, IRow, IIdRow, new()
{
    public RetrieveHandlerProxy(IDefaultHandlerFactory factory) : base(factory) { }
}