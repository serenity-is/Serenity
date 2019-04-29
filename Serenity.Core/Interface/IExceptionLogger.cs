using System;

namespace Serenity.Abstractions
{
    /// <summary>
    /// Exception logger abstraction
    /// </summary>
    public interface IExceptionLogger
    {
        /// <summary>
        /// Logs the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        void Log(Exception exception);
    }
}