using Serenity.Data;

namespace Serenity.Services
{
    public interface ISaveRequestHandler<TRow>
        where TRow : Row, IIdRow, new()
    {
        TRow Old { get; }
        TRow Row { get; }
        IUnitOfWork UnitOfWork { get; }
        SaveRequest<TRow> Request { get; }
        SaveResponse Response { get; }
    }
}