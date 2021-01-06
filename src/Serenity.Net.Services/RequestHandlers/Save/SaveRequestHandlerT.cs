using Serenity.Data;

namespace Serenity.Services
{
    public class SaveRequestHandler<TRow> : SaveRequestHandler<TRow, SaveRequest<TRow>, SaveResponse>
        where TRow : class, IRow, IIdRow, new()
    {
        public SaveRequestHandler(IRequestContext context)
            : base(context)
        {
        }
    }
}
