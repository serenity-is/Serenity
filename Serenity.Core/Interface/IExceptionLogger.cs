using System;

namespace Serenity.Abstractions
{
    public interface IExceptionLogger
    {
        void Log(Exception exception);
    }
}