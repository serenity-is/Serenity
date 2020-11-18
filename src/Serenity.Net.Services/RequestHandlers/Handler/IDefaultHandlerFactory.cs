using System;

namespace Serenity.Services
{
    public interface IDefaultHandlerFactory
    {
        object CreateDefaultHandler(Type rowType, Type handlerType, Type genericType);
    }
}