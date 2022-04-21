namespace Serenity.Services
{
    public interface IDefaultHandlerRegistry
    {
        IEnumerable<Type> GetTypes(Type handlerType);
    }
}