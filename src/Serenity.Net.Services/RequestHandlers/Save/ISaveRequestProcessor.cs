using Serenity.Data;

namespace Serenity.Services
{
    public interface ISaveRequestProcessor : ISaveRequestHandler
    {
        SaveResponse Process(IUnitOfWork uow, ISaveRequest request, SaveRequestType type);
    }
}