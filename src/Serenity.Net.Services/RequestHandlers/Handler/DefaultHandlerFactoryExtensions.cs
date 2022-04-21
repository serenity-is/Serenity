namespace Serenity.Services
{
    public static class DefaultHandlerFactoryExtensions
    {
        public static THandler CreateHandler<THandler>(this IDefaultHandlerFactory handlerFactory, Type rowType)
        {
            return (THandler)handlerFactory.CreateHandler(rowType, typeof(THandler));
        }
    }
}