using Serenity.Data;

namespace Serenity.Services
{
    public class ListRequestHandler<TRow> : ListRequestHandler<TRow, ListRequest, ListResponse<TRow>>
        where TRow : class, IRow, new()
    {
        public ListRequestHandler(IRequestContext context)
            : base(context)
        {
        }
    }
}