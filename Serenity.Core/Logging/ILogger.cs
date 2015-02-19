using Serenity.Logging;
using System;

namespace Serenity.Abstractions
{
    public interface ILogger
    {
        void Write(LoggingLevel level, string message, Exception exception, Type source);
    }
}