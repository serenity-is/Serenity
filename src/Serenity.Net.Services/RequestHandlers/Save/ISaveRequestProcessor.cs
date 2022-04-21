namespace Serenity.Services
{
    [GenericHandlerType(typeof(SaveRequestHandler<>))]
    public interface ISaveRequestProcessor : ISaveRequestHandler
    {
        SaveResponse Process(IUnitOfWork uow, ISaveRequest request, SaveRequestType type);
    }
}