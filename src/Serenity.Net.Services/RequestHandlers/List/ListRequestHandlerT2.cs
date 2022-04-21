namespace Serenity.Services
{
    public class ListRequestHandler<TRow, TListRequest> : ListRequestHandler<TRow, TListRequest, ListResponse<TRow>>,
        IListHandler<TRow, TListRequest>
        where TRow : class, IRow, new()
        where TListRequest : ListRequest
    {
        public ListRequestHandler(IRequestContext context)
            : base(context)
        {
        }
    }
}