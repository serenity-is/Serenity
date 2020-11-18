using Serenity.Data;

namespace Serenity.Services
{
    public class RetrieveRequestHandler<TRow> : RetrieveRequestHandler<TRow, RetrieveRequest, RetrieveResponse<TRow>>
        where TRow : class, IRow, new()
    {
        public RetrieveRequestHandler(IRequestContext context)
            : base(context)
        {
        }
    }
}