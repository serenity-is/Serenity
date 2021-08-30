using Serenity.Data;

namespace Serenity.Services
{
    public interface ISaveHandler<TRow, TSaveRequest, TSaveResponse>
        : ICreateHandler<TRow, TSaveRequest, TSaveResponse>, IUpdateHandler<TRow, TSaveRequest, TSaveResponse>
        where TRow : class, IRow, IIdRow, new()
        where TSaveRequest : SaveRequest<TRow>, new()
        where TSaveResponse : SaveResponse, new()
    {
    }

    public interface ISaveHandler<TRow> : ISaveHandler<TRow, SaveRequest<TRow>, SaveResponse>,
        ICreateHandler<TRow>, IUpdateHandler<TRow>
        where TRow : class, IRow, IIdRow, new()
    {
    }
}