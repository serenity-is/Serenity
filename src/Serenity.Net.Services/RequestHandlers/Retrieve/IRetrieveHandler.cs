namespace Serenity.Services
{
    public interface IRetrieveHandler<TRow, TRetrieveRequest, TRetrieveResponse>
        : IRequestHandler<TRow, TRetrieveRequest, TRetrieveResponse>
        where TRow : class, IRow, new()
        where TRetrieveRequest : RetrieveRequest
        where TRetrieveResponse : RetrieveResponse<TRow>, new()
    {
        TRetrieveResponse Retrieve(IDbConnection connection, TRetrieveRequest request);
    }

    public interface IRetrieveHandler<TRow>
        : IRetrieveHandler<TRow, RetrieveRequest, RetrieveResponse<TRow>>
        where TRow : class, IRow, new()
    {
    }
}