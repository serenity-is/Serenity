namespace Serenity.Services
{
    public interface IListHandler<TRow, TListRequest, TListResponse>
        : IRequestHandler<TRow, TListRequest, TListResponse>
        where TRow : class, IRow, new()
        where TListRequest : ListRequest
        where TListResponse : ListResponse<TRow>, new()
    {
        TListResponse List(IDbConnection connection, TListRequest request);
    }

    public interface IListHandler<TRow, TListRequest>
        : IListHandler<TRow, TListRequest, ListResponse<TRow>>
        where TRow : class, IRow, new()
        where TListRequest : ListRequest
    {
    }

    public interface IListHandler<TRow>
        : IListHandler<TRow, ListRequest, ListResponse<TRow>>
        where TRow : class, IRow, new()
    {
    }
}