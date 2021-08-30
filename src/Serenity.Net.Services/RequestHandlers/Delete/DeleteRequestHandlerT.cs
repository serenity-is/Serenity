using Serenity.Data;

namespace Serenity.Services
{
    public class DeleteRequestHandler<TRow> : DeleteRequestHandler<TRow, DeleteRequest, DeleteResponse>,
        IDeleteHandler<TRow>
        where TRow : class, IRow, IIdRow, new()
    {
        public DeleteRequestHandler(IRequestContext context)
            : base(context)
        {
        }
    }
}