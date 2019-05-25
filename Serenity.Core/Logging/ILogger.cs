using Serenity.Logging;
using System;

namespace Serenity.Abstractions
{
    /// <summary>
    /// Logger abstraction
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Writes the specified level log message.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="source">The source.</param>
        void Write(LoggingLevel level, string message, Exception exception, Type source);
    }
}