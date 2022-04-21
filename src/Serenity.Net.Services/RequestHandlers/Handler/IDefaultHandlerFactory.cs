namespace Serenity.Services
{
    public interface IDefaultHandlerFactory
    {
        object CreateHandler(Type rowType, Type handlerInterface);
    }
}