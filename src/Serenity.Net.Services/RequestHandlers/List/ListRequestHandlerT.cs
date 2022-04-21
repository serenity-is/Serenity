namespace Serenity.Services
{
    public class ListRequestHandler<TRow> : ListRequestHandler<TRow, ListRequest, ListResponse<TRow>>,
        IListHandler<TRow>, IListHandler<TRow, ListRequest>
        where TRow : class, IRow, new()
    {
        public ListRequestHandler(IRequestContext context)
            : base(context)
        {
        }
    }
}