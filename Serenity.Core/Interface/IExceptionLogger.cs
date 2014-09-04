using System;

namespace Serenity.Services
{
    public interface IExceptionLogger
    {
        void Log(Exception exception);
    }
}