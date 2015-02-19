using System;
using Serenity.Abstractions;

namespace Serenity
{
    public interface INotLoggedException
    {
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
