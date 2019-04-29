using System;
using Serenity.Abstractions;

namespace Serenity
{
    /// <summary>
    /// An exception that should not be logged.
    /// </summary>
    public interface INotLoggedException
    {
        /// <summary>
        /// Gets a value indicating whether to not log exception.
        /// </summary>
        /// <value>
        ///   <c>true</c> if not logged exception; otherwise, <c>false</c>.
        /// </value>
        bool NotLoggedException { get; }
    }

    /// <summary>
    ///   Centralized logger for exceptions.</summary>
    public static class ExceptionLog
    {
        /// <summary>
        ///   Logs an exception. Nothing logged if exception logger delegate is not set.</summary>
        /// <param name="e"></param>
        public static void Log(this Exception e)
        {
            try
            {
                var exceptionLogger = Dependency.TryResolve<IExceptionLogger>();
                if (exceptionLogger != null)
                {
                    var no = e as INotLoggedException;
                    if (no == null ||
                        !no.NotLoggedException)
                        exceptionLogger.Log(e);
                }
            }
            catch
            {
            }
        }
    }
}
