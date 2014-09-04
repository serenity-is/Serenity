using Serenity.Data;

namespace Serenity.Services
{
    public interface ISaveRequestHandler
    {
        Row Old { get; }
        Row Row { get; }
        IUnitOfWork UnitOfWork { get; }
        ISaveRequest Request { get; }
        SaveResponse Response { get; }
    }
}