using System;

namespace Serenity.Services
{
    public interface IHandlerActivator
    {
        object CreateInstance(Type type);
    }
}